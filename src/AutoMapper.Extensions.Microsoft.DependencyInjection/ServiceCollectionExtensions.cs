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

            var profiles =
                assembliesToScan.SelectMany(a => a.ExportedTypes).Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()));

            Mapper.Initialize(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
        }

    }
}