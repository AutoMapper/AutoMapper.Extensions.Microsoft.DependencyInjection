using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoMapper.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions to scan for AutoMapper classes and register the configuration, mapping, and extensions with the service collection:
    /// <list type="bullet">
    /// <item> Finds <see cref="Profile"/> classes and initializes a new <see cref="MapperConfiguration" />,</item> 
    /// <item> Scans for <see cref="ITypeConverter{TSource,TDestination}"/>, <see cref="IValueResolver{TSource,TDestination,TDestMember}"/>, <see cref="IMemberValueResolver{TSource,TDestination,TSourceMember,TDestMember}" /> and <see cref="IMappingAction{TSource,TDestination}"/> implementations and registers them as <see cref="ServiceLifetime.Transient"/>, </item>
    /// <item> Registers <see cref="IConfigurationProvider"/> as <see cref="ServiceLifetime.Singleton"/>, and</item>
    /// <item> Registers <see cref="IMapper"/> as a configurable <see cref="ServiceLifetime"/> (default is <see cref="ServiceLifetime.Transient"/>)</item>
    /// </list>
    /// After calling AddAutoMapper you can resolve an <see cref="IMapper" /> instance from a scoped service provider, or as a dependency
    /// To use <see cref="QueryableExtensions.Extensions.ProjectTo{TDestination}(IQueryable,IConfigurationProvider, System.Linq.Expressions.Expression{System.Func{TDestination, object}}[])" /> you can resolve the <see cref="IConfigurationProvider"/> instance directly for from an <see cref="IMapper" /> instance.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
            => AddAutoMapperClasses(services, null, assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, params Assembly[] assemblies)
            => AddAutoMapperClasses(services, (sp, cfg) => configAction?.Invoke(cfg), assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, params Assembly[] assemblies)
            => AddAutoMapperClasses(services, configAction, assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, IEnumerable<Assembly> assemblies, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            => AddAutoMapperClasses(services, (sp, cfg) => configAction?.Invoke(cfg), assemblies, serviceLifetime);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, IEnumerable<Assembly> assemblies, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            => AddAutoMapperClasses(services, configAction, assemblies, serviceLifetime);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            => AddAutoMapperClasses(services, null, assemblies, serviceLifetime);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Type[] profileAssemblyMarkerTypes)
            => AddAutoMapperClasses(services, null, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, params Type[] profileAssemblyMarkerTypes)
            => AddAutoMapperClasses(services, (sp, cfg) => configAction?.Invoke(cfg), profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, params Type[] profileAssemblyMarkerTypes)
            => AddAutoMapperClasses(services, configAction, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction,
            IEnumerable<Type> profileAssemblyMarkerTypes, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            => AddAutoMapperClasses(services, (sp, cfg) => configAction?.Invoke(cfg), profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly), serviceLifetime);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction,
            IEnumerable<Type> profileAssemblyMarkerTypes, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            => AddAutoMapperClasses(services, configAction, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly), serviceLifetime);

        private static IServiceCollection AddAutoMapperClasses(IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction,
            IEnumerable<Assembly> assembliesToScan, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            // Just return if we've already added AutoMapper to avoid double-registration
            if (services.Any(sd => sd.ServiceType == typeof(IMapper)))
                return services;

            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            var allTypes = assembliesToScan
                .Where(a => !a.IsDynamic && a.GetName().Name != nameof(AutoMapper))
                .Distinct() // avoid AutoMapper.DuplicateTypeMapConfigurationException
                .SelectMany(a => a.DefinedTypes)
                .ToArray();

            void ConfigAction(IServiceProvider serviceProvider, IMapperConfigurationExpression cfg)
            {
                configAction?.Invoke(serviceProvider, cfg);

                cfg.AddMaps((type) => serviceProvider.AsSelf(type), assembliesToScan);
            }

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IValueConverter<,>),
                typeof(IMappingAction<,>)
            };
            foreach (var type in openTypes.SelectMany(openType => allTypes
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.AsType().ImplementsGenericInterface(openType))))
            {
                services.AddTransient(type.AsType());
                // It is also useful to be able to resolve the type the closed interface as well. 
                foreach (var @interface in type.GetInterfaces())
                {
                    if (@interface.IsGenericType && openTypes.Any(openType => openType == @interface.GetGenericTypeDefinition()))
                    {
                        services.AddTransient(@interface, type);
                    }
                }
            }

            services.AddSingleton<IConfigurationProvider>(sp => new MapperConfiguration(cfg => ConfigAction(sp, cfg)));
            services.Add(new ServiceDescriptor(typeof(IMapper),
                sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService), serviceLifetime));

            return services;
        }

        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
            => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        private static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;

        /// <summary>
        /// Method MappingExpression.ForMember(MemberInfo destinationProperty, Action{IMemberConfigurationExpression} memberOptions) 
        /// and class MemberConfigurationExpression are marked internal. To workaround this use reflection to get the method.
        /// https://github.com/AutoMapper/AutoMapper/blob/574593b809c69c23f1d7be411e1492ba1a585467/src/AutoMapper/Configuration/MappingExpression.cs#L84-L95
        /// </summary>
        private static readonly MethodInfo ForMember = typeof(MappingExpression)
            .GetMethod(
                name: nameof(MappingExpression.ForMember),
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: new Type[] { typeof(MemberInfo), typeof(Action<IMemberConfigurationExpression>) },
                modifiers: null)
            ;

        /// <remarks>
        /// Class <see cref="NamedProfile"/> is marked private in AutoMapper. By duplicating that class we can maintain functionality. 
        /// https://github.com/AutoMapper/AutoMapper/blob/df1b5fe13d64f11885746291987aca89fe808ee0/src/AutoMapper/Configuration/MapperConfigurationExpression.cs#L33-L42
        /// </remarks>
        private class NamedProfile : Profile
        {
            public NamedProfile(string profileName) : base(profileName)
            {
            }

            public NamedProfile(string profileName, Action<IProfileExpression> config) : base(profileName, config)
            {
            }
        }


        /// <summary>
        /// Allows <see cref="Profile"/>s to be instantiated from an IoC container.
        /// This works around the use of <see cref="System.Activator.CreateInstance(Type)"/> in 
        /// <see cref="AutoMapper.Configuration.MapperConfigurationExpression"/>. 
        /// https://github.com/AutoMapper/AutoMapper/blob/df1b5fe13d64f11885746291987aca89fe808ee0/src/AutoMapper/Configuration/MapperConfigurationExpression.cs#L51
        /// </summary>
        /// <remarks>
        /// Ideally an overload of AddMaps with this signature would exist on IMapperConfigurationExpression and MapperConfigurationExpression. This
        /// would allow code that was duplicated to be removed. This follows the established pattern used by <see cref="Mapper"/>. 
        /// </remarks>
        private static void AddMaps(this IMapperConfigurationExpression configuration, Func<Type, object> serviceCtor, IEnumerable<Assembly> assemblies)
        {
            var allTypes = assemblies
                .Where(assembly => !assembly.IsDynamic &&
                    assembly != typeof(AutoMapper.IMapper).Assembly &&
                    assembly != typeof(NamedProfile).Assembly)
                .SelectMany(assembly => assembly.DefinedTypes)
                .ToArray();
            var autoMapAttributeProfile = new NamedProfile(nameof(AutoMapAttribute));

            foreach (var type in allTypes)
            {
                if (typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    // Instead of creating the Profile using Activator.CreateInstance use the IServiceProvider. 
                    // This allows Profiles to declare dependencies and have them fulfilled via injection.
                    var profile = (Profile)serviceCtor(type);
                    if (profile == null)
                    {
                        throw new InvalidOperationException($"Profile {type.FullName} could not be created. Ensure all dependencies have been registered and a usable constructor exists.");
                    }
                    configuration.AddProfile(profile);
                }
                foreach (var autoMapAttribute in type.GetCustomAttributes<AutoMapAttribute>())
                {
                    var mappingExpression = (MappingExpression)autoMapAttributeProfile.CreateMap(autoMapAttribute.SourceType, type);
                    autoMapAttribute.ApplyConfiguration(mappingExpression);

                    foreach (var memberInfo in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                    {
                        foreach (var memberConfigurationProvider in memberInfo.GetCustomAttributes().OfType<IMemberConfigurationProvider>())
                        {
                            Action<IMemberConfigurationExpression> memberOptions = cfg => memberConfigurationProvider.ApplyConfiguration(cfg);
                            ForMember.Invoke(mappingExpression, new object[] { memberInfo, memberOptions });
                        }
                    }
                }
            }

            configuration.AddProfile(autoMapAttributeProfile);
        }
    }
}
