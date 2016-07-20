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
#if DEPENDENCY_MODEL
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(DependencyContext.Default);
        }

        public static void AddAutoMapper(this IServiceCollection services, DependencyContext dependencyContext)
        {
            services.AddAutoMapper(dependencyContext.RuntimeLibraries
                .SelectMany(lib => lib.GetDefaultAssemblyNames(dependencyContext).Select(Assembly.Load)));
        }
#endif

        public static void AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            AddAutoMapperClasses(services, assemblies);
        }

        public static void AddAutoMapper(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            AddAutoMapperClasses(services, assemblies);
        }

        public static void AddAutoMapper(this IServiceCollection services, params Type[] profileAssemblyMarkerTypes)
        {
            AddAutoMapperClasses(services, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }

        public static void AddAutoMapper(this IServiceCollection services, IEnumerable<Type> profileAssemblyMarkerTypes)
        {
            AddAutoMapperClasses(services, profileAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }


        private static void AddAutoMapperClasses(IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            var allTypes = assembliesToScan.SelectMany(a => a.ExportedTypes).ToArray();

            var profiles =
                allTypes
                    .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
                    .Where(t => !t.GetTypeInfo().IsAbstract);

            Mapper.Initialize(cfg =>
            {
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
                foreach (var type in allTypes.Where(t => t.ImplementsGenericInterface(openType)))
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