namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using AutoMapper.Extensions.Microsoft.DependencyInjection;
    using AutoMapper.Extensions.Microsoft.DependencyInjection.Internal;
    using System.ComponentModel;

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
        public static IAutoMapperBuilder AddAutoMapper(this IServiceCollection services)
        {
            return AddAutoMapperInternal(services, null);
        }

        public static IAutoMapperBuilder AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction)
        {
            if(configAction != null)
            {
                return AddAutoMapperInternal(services, (sp, cfg) => configAction.Invoke(cfg));
            }
            else
            {
                return AddAutoMapperInternal(services, null);
            }
            
        }

        //public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction)
        //    => services.AddAutoMapper(configAction, AppDomain.CurrentDomain.GetAssemblies());

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services);
            builder.AddAssemblies(assemblies);
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, params Assembly[] assemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services, configAction);
            builder.AddAssemblies(assemblies);
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, params Assembly[] assemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services, configAction);
            builder.AddAssemblies(assemblies);
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, IEnumerable<Assembly> assemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services, configAction);
            builder.AddAssemblies(assemblies);
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, IEnumerable<Assembly> assemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services, configAction);
            builder.AddAssemblies(assemblies);
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services);
            builder.AddAssemblies(assemblies);
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Type[] profileAssemblyMarkerTypes)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services);
            builder.AddAssemblies(profileAssemblyMarkerTypes.Select(t=>t.Assembly));
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, params Type[] profileAssemblyMarkerTypes)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services,configAction);
            builder.AddAssemblies(profileAssemblyMarkerTypes.Select(t => t.Assembly));
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, params Type[] profileAssemblyMarkerTypes)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services, configAction);
            builder.AddAssemblies(profileAssemblyMarkerTypes.Select(t => t.Assembly));
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, IEnumerable<Type> profileAssemblyMarkerTypes)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services, configAction);
            builder.AddAssemblies(profileAssemblyMarkerTypes.Select(t => t.Assembly));
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use other method and  returned builder to add Assmblies please")]
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, IEnumerable<Type> profileAssemblyMarkerTypes)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = AddAutoMapper(services, configAction);
            builder.AddAssemblies(profileAssemblyMarkerTypes.Select(t => t.Assembly));
            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("no longer used",true)]
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
                typeof(IValueConverter<,>),
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



        public static IAutoMapperBuilder AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction)
        {
            return AddAutoMapperInternal(services, configAction);
        }

        public static IAutoMapperBuilder AddAutoMapperInternal(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            if (configAction != null)
            {
                services.AddSingleton<IPostAutoMapperConfiguration>(new PostAutoMapperConfiguration(configAction));
            }
            services.TryAddSingleton<IAutoMapperConfigurationProvider, AutoMapperConfigurationProvider>();
            services.TryAddSingleton<IConfigurationProvider>(sp =>
            {
                var postConfigs = sp.GetRequiredService<IEnumerable<IPostAutoMapperConfiguration>>();//should be Singletion
                var mapConfigProvider = sp.GetRequiredService<IAutoMapperConfigurationProvider>();//should be Singletion
                return new MapperConfiguration(cfg =>
                {
                    foreach(var t in mapConfigProvider.GetMapProfileTypes())
                    {
                        cfg.AddProfile(t);
                    }
                    //cfg.AddProfiles();
                    foreach (var config in postConfigs)
                    {
                        config?.Configuration(sp, cfg);
                    }
                });
            });
            services.TryAddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
            var builder = new AutoMapperBuilder(services);
            return builder;
        }

        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
            => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        private static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
