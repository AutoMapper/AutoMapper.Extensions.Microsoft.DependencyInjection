
namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class AssemblyExtensions
    {
        public static IEnumerable<TypeInfo> GetAllTypes(this IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .Where(a => a.GetName().Name != nameof(AutoMapper))
                .SelectMany(a => a.DefinedTypes);
        }
    }
}