using System.Collections;

namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
#if DEPENDENCY_MODEL
    using Microsoft.Extensions.DependencyModel;
#endif

    /// <summary>
    /// Extensions to scan for AutoMapper classes and register them with the static/singleton Mapper class
    /// - Finds <see cref="Profile"/> classes and initializes AutoMapper with them using <see cref="Mapper.Initialize(Action{AutoMapper.IMapperConfigurationExpression})"/>
    /// - Scans for <see cref="ITypeConverter{TSource,TDestination}"/>, <see cref="IValueResolver{TSource,TDestination,TDestMember}"/> and <see cref="IMemberValueResolver{TSource,TDestination,TSourceMember,TDestMember}" /> implementations and registers them as <see cref="ServiceLifetime.Transient"/>
    /// - Registers <see cref="Mapper.Configuration"/> as <see cref="ServiceLifetime.Singleton"/>
    /// - Registers <see cref="IMapper"/> as <see cref="ServiceLifetime.Scoped"/> with a service factory of the scoped <see cref="IServiceProvider"/>
    /// After calling AddAutoMapper you will have the static <see cref="Mapper"/> configuration initialized and you can use Mapper.Map and ProjectTo in your application code.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
#if DEPENDENCY_MODEL
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            return services.AddAutoMapper(null, DependencyContext.Default);
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction)
        {
            return services.AddAutoMapper(additionalInitAction, DependencyContext.Default);
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, DependencyContext dependencyContext)
        {
            return services.AddAutoMapper(additionalInitAction, new CandidateResolver(dependencyContext).GetCandidateAssemblies());
        }
#endif

        private static readonly Action<IMapperConfigurationExpression> DefaultConfig = cfg => { };

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
            => AddAutoMapperClasses(services, null, assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, params Assembly[] assemblies) 
            => AddAutoMapperClasses(services, additionalInitAction, assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Assembly> assemblies) 
            => AddAutoMapperClasses(services, additionalInitAction, assemblies);

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Type[] profileAssemblyMarkerTypes)
        {
            return AddAutoMapperClasses(services, null, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, params Type[] profileAssemblyMarkerTypes)
        {
            return AddAutoMapperClasses(services, additionalInitAction, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Type> profileAssemblyMarkerTypes)
        {
            return AddAutoMapperClasses(services, additionalInitAction, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }


        private static IServiceCollection AddAutoMapperClasses(IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Assembly> assembliesToScan)
        {
            additionalInitAction = additionalInitAction ?? DefaultConfig;
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            var allTypes = assembliesToScan
                .Where(a => a.GetName().Name != nameof(AutoMapper))
                .SelectMany(a => a.DefinedTypes)
                .ToArray();

            var profiles =
                allTypes
                    .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t))
                    .Where(t => !t.IsAbstract);

            Mapper.Initialize(cfg =>
            {
                additionalInitAction(cfg);

                foreach (var profile in profiles.Select(t => t.AsType()))
                {
                    cfg.AddProfile(profile);
                }
            });

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>)
            };
            foreach (var type in openTypes.SelectMany(openType => allTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => t.AsType().ImplementsGenericInterface(openType))))
            {
                services.AddTransient(type.AsType());
            }

            services.AddSingleton(Mapper.Configuration);
            return services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
        }

        private static readonly IList<Action<IMapperConfigurationExpression>> StaticConfigurationExpressionActions =
            new List<Action<IMapperConfigurationExpression>>();

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<MapperBuilder> builderAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (builderAction != null)
            {
                var builder = new MapperBuilder(services);
                builderAction(builder);

                bool useStaticInitialization = ((IMapperBuilder) builder).UseStaticInitialization;
                foreach (var configurationExpressionAction in ((IMapperBuilder) builder).ConfigurationExpressionActions)
                {
                    services.AddTransient<IMapperConfigurationExpressionAction>(
                        sp => new MapperConfigurationExpressionAction(configurationExpressionAction));

                    if (useStaticInitialization)
                    {
                        // lock the collection because running unit tests in parallel could concurrently access statics
                        lock (((ICollection)StaticConfigurationExpressionActions).SyncRoot)
                            StaticConfigurationExpressionActions.Add(configurationExpressionAction);
                    }
                }

                if (useStaticInitialization)
                {
                    lock (((ICollection)StaticConfigurationExpressionActions).SyncRoot)
                    {
                        Mapper.Initialize(config =>
                        {
                            foreach (var staticConfigAction in StaticConfigurationExpressionActions)
                                staticConfigAction(config);
                        });
                    }
                }
            }

            services.TryAddSingleton<IConfigurationProvider>(sp =>
            {
                var configActions = sp.GetServices<IMapperConfigurationExpressionAction>();
                return new MapperConfiguration(config =>
                {
                    foreach (var configAction in configActions)
                        configAction.Action(config);
                });
            });

            services.TryAddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
            return services;
        }
    }
}