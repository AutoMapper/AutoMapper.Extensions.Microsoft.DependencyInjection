namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
#if DEPENDENCY_MODEL
    using Microsoft.Extensions.DependencyModel;
#endif

    public static class ProfileMapperBuilderExtensions
    {
        public static MapperBuilder AddProfiles(this MapperBuilder builder, IEnumerable<Assembly> assembliesToScan)
        {
            return builder.UseConfiguration(c => c.AddProfiles(assembliesToScan));
        }

        public static MapperBuilder AddProfiles(this MapperBuilder builder, params Assembly[] assembliesToScan)
        {
            return builder.UseConfiguration(c => c.AddProfiles(assembliesToScan));
        }

        public static MapperBuilder AddProfiles(this MapperBuilder builder, IEnumerable<Type> profileAssemblyMarkerTypes)
        {
            return builder.UseConfiguration(c => c.AddProfiles(profileAssemblyMarkerTypes));
        }

        public static MapperBuilder AddProfiles(this MapperBuilder builder, params Type[] profileAssemblyMarkerTypes)
        {
            return builder.UseConfiguration(c => c.AddProfiles(profileAssemblyMarkerTypes));
        }

#if DEPENDENCY_MODEL
        public static MapperBuilder AddProfiles(this MapperBuilder builder)
        {
            return builder.AddProfiles(DependencyContext.Default);
        }

        public static MapperBuilder AddProfiles(this MapperBuilder builder, DependencyContext dependencyContext)
        {
            return builder.AddProfiles(new CandidateResolver(dependencyContext).GetCandidateAssemblies());
        }
#endif
    }
}