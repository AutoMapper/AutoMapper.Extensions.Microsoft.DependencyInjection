using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    public static class AssemblyScanHelper
    {

        public static IEnumerable<Type> GetRelatedTypes(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => ScanAssembly(a, typeof(IValueResolver<,,>)))
                .Union(assemblies.SelectMany(a => ScanAssembly(a, typeof(IMemberValueResolver<,,,>))))
                .Union(assemblies.SelectMany(a => ScanAssembly(a, typeof(ITypeConverter<,>))))
                .Union(assemblies.SelectMany(a => ScanAssembly(a, typeof(IValueConverter<,>))))
                .Union(assemblies.SelectMany(a => ScanAssembly(a, typeof(IMappingAction<,>))))
                .ToList();
        }

        public static IEnumerable<Type> ScanAssembly(this Assembly assembly, Type targetType)
        {
            if (assembly == typeof(IMapper).Assembly)
            {
                return Enumerable.Empty<Type>();
            }
            var types = assembly.DefinedTypes.Where(t => t != targetType && t.IsClass && !t.IsAbstract);
            if (targetType.IsGenericType)
            {
                if (targetType.IsInterface)
                {
                    return types.Where(t =>
                    {
                        var b = t.ImplementedInterfaces.Where(g => g.IsGenericType).Select(g => g.GetGenericTypeDefinition()).Contains(targetType);
                        return b;
                    });
                }
                else
                {
                    return types.Where(t => t.GetGenericTypeDefinition().IsSubclassOf(targetType));
                }
            }
            else
            {
                return types.Where(t => targetType.IsAssignableFrom(t));
            }

        }
    }
}
