namespace AutoMapper
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

#if DEPENDENCY_MODEL
    using Microsoft.Extensions.DependencyModel;
#endif

    public static class TypeConverterMapperBuilderExtensions
    {
        public static MapperBuilder AddTypeConverters(this MapperBuilder builder, IEnumerable<Assembly> assemblies)
        {
            var services = ((IMapperBuilder)builder).Services;

            var interfaceTypes = new[]
            {
                typeof(ITypeConverter<,>)
            };

            var typeConverterTypes = assemblies.GetAllTypes()
                .GetTypesImplementingGenericInterface(interfaceTypes);

            foreach (var type in typeConverterTypes)
            {
                services.TryAddTransient(type.AsType());
            }

            return builder;
        }

        public static MapperBuilder AddTypeConverters(this MapperBuilder builder, params Assembly[] assemblies)
        {
            return builder.AddTypeConverters((IEnumerable<Assembly>) assemblies);
        }

        public static MapperBuilder AddTypeConverters(this MapperBuilder builder, IEnumerable<Type> assemblyMarkerTypes)
        {
            return builder.AddTypeConverters(assemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly).Distinct());
        }

        public static MapperBuilder AddTypeConverters(this MapperBuilder builder, params Type[] assemblyMarkerTypes)
        {
            return builder.AddTypeConverters((IEnumerable<Type>)assemblyMarkerTypes);
        }

#if DEPENDENCY_MODEL
        public static MapperBuilder AddTypeConverters(this MapperBuilder builder)
        {
            return builder.AddTypeConverters(DependencyContext.Default);
        }

        public static MapperBuilder AddTypeConverters(this MapperBuilder builder, DependencyContext dependencyContext)
        {
            return builder.AddTypeConverters(new CandidateResolver(dependencyContext).GetCandidateAssemblies());
        }
#endif
    }
}