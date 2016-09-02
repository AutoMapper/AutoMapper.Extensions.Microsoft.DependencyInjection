namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
#if DEPENDENCY_MODEL
    using Microsoft.Extensions.DependencyModel;
#endif

    public static class ServiceCollectionExtensions
    {
        private static readonly Assembly AutoMapperAssembly = typeof(Mapper).GetTypeInfo().Assembly;

        private static readonly Action<IMapperConfigurationExpression> DefaultConfig = cfg => { };
#if DEPENDENCY_MODEL
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(null, DependencyContext.Default);
        }

        public static void AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction)
        {
            services.AddAutoMapper(additionalInitAction, DependencyContext.Default);
        }

        public static void AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, DependencyContext dependencyContext)
        {
            services.AddAutoMapper(additionalInitAction,
                dependencyContext.RuntimeLibraries
                    // Only load assemblies that reference AutoMapper
                    .Where(lib =>
                        lib.Type.Equals("msbuildproject", StringComparison.OrdinalIgnoreCase) ||
                        lib.Dependencies.Any(d => d.Name.StartsWith(AutoMapperAssembly.GetName().Name, StringComparison.OrdinalIgnoreCase)))
                    .SelectMany(lib => lib.GetDefaultAssemblyNames(dependencyContext)
                        .Select(Assembly.Load)));
        }
#endif

        public static void AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            AddAutoMapperClasses(services, null, assemblies);
        }

        public static void AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, params Assembly[] assemblies)
        {
            AddAutoMapperClasses(services, additionalInitAction, assemblies);
        }

        public static void AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Assembly> assemblies)
        {
            AddAutoMapperClasses(services, additionalInitAction, assemblies);
        }

        public static void AddAutoMapper(this IServiceCollection services, params Type[] profileAssemblyMarkerTypes)
        {
            AddAutoMapperClasses(services, null, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }

        public static void AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, params Type[] profileAssemblyMarkerTypes)
        {
            AddAutoMapperClasses(services, additionalInitAction, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }

        public static void AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Type> profileAssemblyMarkerTypes)
        {
            AddAutoMapperClasses(services, additionalInitAction, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }


        private static void AddAutoMapperClasses(IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Assembly> assembliesToScan)
        {
            additionalInitAction = additionalInitAction ?? DefaultConfig;
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            var allTypes = assembliesToScan.SelectMany(a => a.ExportedTypes).ToArray();

            var profiles =
                allTypes
                    .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
                    .Where(t => !t.GetTypeInfo().IsAbstract);

            Mapper.Initialize(cfg =>
            {
                additionalInitAction(cfg);

                foreach (var profile in profiles)
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
            foreach (var openType in openTypes)
            {
                foreach (var type in allTypes
                    .Where(t => t.GetTypeInfo().IsClass)
                    .Where(t => !t.GetTypeInfo().IsAbstract)
                    .Where(t => t.ImplementsGenericInterface(openType)))
                {
                    services.AddTransient(type);
                }
            }

            services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
        }

        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            if (type.IsGenericType(interfaceType))
            {
                return true;
            }
            foreach (var @interface in type.GetTypeInfo().ImplementedInterfaces)
            {
                if (@interface.IsGenericType(interfaceType))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsGenericType(this Type type, Type genericType)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }


    }
}