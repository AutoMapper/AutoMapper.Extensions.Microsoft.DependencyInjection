namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions to scan for AutoMapper classes and register the configuration, mapping, and extensions with the service collection
    /// - Finds <see cref="Profile"/> classes and initializes a new <see cref="MapperConfiguration" />
    /// - Scans for <see cref="ITypeConverter{TSource,TDestination}"/>, <see cref="IValueResolver{TSource,TDestination,TDestMember}"/>, <see cref="IMemberValueResolver{TSource,TDestination,TSourceMember,TDestMember}" /> and <see cref="IMappingAction{TSource,TDestination}"/> implementations and registers them as <see cref="ServiceLifetime.Transient"/>
    /// - Registers <see cref="IConfigurationProvider"/> as <see cref="ServiceLifetime.Singleton"/>
    /// - Registers <see cref="IMapper"/> as <see cref="ServiceLifetime.Scoped"/> with a service factory of the scoped <see cref="IServiceProvider"/>
    /// After calling AddAutoMapper you can resolve an <see cref="IMapper" /> instance from a scoped service provider, or as a dependency
    /// To use <see cref="QueryableExtensions.Extensions.ProjectTo{TDestination}(IQueryable,IConfigurationProvider, System.Linq.Expressions.Expression{System.Func{TDestination, object}}[])" /> you can resolve the <see cref="IConfigurationProvider"/> instance directly for from an <see cref="IMapper" /> instance.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            return services.AddAutoMapper((Action<IServiceProvider, IMapperConfigurationExpression>)null, AppDomain.CurrentDomain.GetAssemblies());
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction)
            => services.AddAutoMapper((sp, cfg) => configAction?.Invoke(cfg), AppDomain.CurrentDomain.GetAssemblies());

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction)
            => services.AddAutoMapper(configAction, AppDomain.CurrentDomain.GetAssemblies());

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
            => AddAutoMapperClasses(services, null, assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, params Assembly[] assemblies) 
            => AddAutoMapperClasses(services, (sp, cfg) => configAction?.Invoke(cfg), assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, params Assembly[] assemblies)
            => AddAutoMapperClasses(services, configAction, assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, IEnumerable<Assembly> assemblies)
            => AddAutoMapperClasses(services, (sp, cfg) => configAction?.Invoke(cfg), assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, IEnumerable<Assembly> assemblies) 
            => AddAutoMapperClasses(services, configAction, assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, IEnumerable<Assembly> assemblies)
            => AddAutoMapperClasses(services, null, assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Type[] profileAssemblyMarkerTypes)
            => AddAutoMapperClasses(services, null, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, params Type[] profileAssemblyMarkerTypes)
            => AddAutoMapperClasses(services, (sp, cfg) => configAction?.Invoke(cfg), profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, params Type[] profileAssemblyMarkerTypes)
            => AddAutoMapperClasses(services, configAction, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, IEnumerable<Type> profileAssemblyMarkerTypes)
            => AddAutoMapperClasses(services, (sp, cfg) => configAction?.Invoke(cfg), profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, IEnumerable<Type> profileAssemblyMarkerTypes)
            => AddAutoMapperClasses(services, configAction, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));

        private static IServiceCollection AddAutoMapperClasses(IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, IEnumerable<Assembly> assembliesToScan)
        {
            // Just return if we've already added AutoMapper to avoid double-registration
            if (services.Any(sd => sd.ServiceType == typeof(IMapper)))
                return services;

            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            var allTypes = assembliesToScan
                .Where(a => a.GetName().Name != nameof(AutoMapper))
                .SelectMany(a => a.DefinedTypes)
                .ToArray();

            var profileTypeInfo = typeof(Profile).GetTypeInfo();
            var profiles = allTypes
                .Where(t => profileTypeInfo.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();

            void ConfigAction(IServiceProvider serviceProvider, IMapperConfigurationExpression cfg)
            {
                configAction?.Invoke(serviceProvider, cfg);

                foreach (var profile in profiles.Select(t => t.AsType()))
                {
                    cfg.AddProfile(profile);
                }
            }

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IMappingAction<,>)
            };
            foreach (var type in openTypes.SelectMany(openType => allTypes
                .Where(t => t.IsClass 
                    && !t.IsAbstract 
                    && t.AsType().ImplementsGenericInterface(openType))))
            {
                services.AddTransient(type.AsType());
            }

            services.AddSingleton<IConfigurationProvider>(sp => new MapperConfiguration(cfg => ConfigAction(sp, cfg)));
            return services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
        }

        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
            => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        private static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
