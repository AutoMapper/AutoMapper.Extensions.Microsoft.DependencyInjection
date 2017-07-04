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

    public static class ValueResolverMapperBuilderExtensions
    {
        public static MapperBuilder AddValueResolvers(this MapperBuilder builder, IEnumerable<Assembly> assemblies)
        {
            var services = ((IMapperBuilder) builder).Services;

            var interfaceTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>)
            };

            var valueResolverTypes = assemblies.GetAllTypes()
                .GetTypesImplementingGenericInterface(interfaceTypes);

            foreach (var type in valueResolverTypes)
            {
                services.TryAddTransient(type.AsType());
            }

            return builder;
        }

        public static MapperBuilder AddValueResolvers(this MapperBuilder builder, params Assembly[] assemblies)
        {
            return builder.AddValueResolvers((IEnumerable<Assembly>) assemblies);
        }

        public static MapperBuilder AddValueResolvers(this MapperBuilder builder, IEnumerable<Type> assemblyMarkerTypes)
        {
            return builder.AddValueResolvers(assemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly).Distinct());
        }

        public static MapperBuilder AddValueResolvers(this MapperBuilder builder, params Type[] assemblyMarkerTypes)
        {
            return builder.AddValueResolvers((IEnumerable<Type>)assemblyMarkerTypes);
        }

#if DEPENDENCY_MODEL
        public static MapperBuilder AddValueResolvers(this MapperBuilder builder)
        {
            return builder.AddValueResolvers(DependencyContext.Default);
        }

        public static MapperBuilder AddValueResolvers(this MapperBuilder builder, DependencyContext dependencyContext)
        {
            return builder.AddValueResolvers(new CandidateResolver(dependencyContext).GetCandidateAssemblies());
        }
#endif
    }
}
