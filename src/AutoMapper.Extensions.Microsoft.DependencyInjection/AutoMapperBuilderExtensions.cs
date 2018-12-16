using AutoMapper;
using AutoMapper.Extensions.Microsoft.DependencyInjection;
using AutoMapper.Extensions.Microsoft.DependencyInjection.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutoMapperBuilderExtensions
    {
        public static IAutoMapperBuilder AddAssembly(this IAutoMapperBuilder builder, Assembly assembly)
        {
            CheckNull(builder);
            CheckAssemblyNull(assembly);

            AddAssemblies(builder, new[] { assembly });

            return builder;
        }

        public static IAutoMapperBuilder AddAssemblies(this IAutoMapperBuilder builder, params Assembly[] assemblies)
        {
            AddAssemblies(builder, (IEnumerable<Assembly>)assemblies);
            return builder;
        }

        public static IAutoMapperBuilder AddAssemblies(this IAutoMapperBuilder builder, IEnumerable<Assembly> assemblies)
        {
            CheckNull(builder);
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }
            builder.ServiceDescriptor.AddSingleton<IAutoMapperAssemblyDescription>(new AutoMapperAssemblyDescription(assemblies));

            var relatedTypes = GetRelatedTypes(assemblies);
            foreach (var t in relatedTypes)
            {
                builder.ServiceDescriptor.TryAddTransient(t);
            }
            return builder;
        }


        public static IAutoMapperBuilder AddProfileType(this IAutoMapperBuilder builder, Type profileType)
        {
            AddProfileTypes(builder, new[] { profileType });

            return builder;
        }

        public static IAutoMapperBuilder AddProfileType<T>(this IAutoMapperBuilder builder) where T : Profile
        {
            return AddProfileType(builder, typeof(T));
        }

        public static IAutoMapperBuilder AddProfileTypes(this IAutoMapperBuilder builder, IEnumerable<Type> profileTypes)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (profileTypes == null)
            {
                throw new ArgumentNullException(nameof(profileTypes));
            }
            var ptype = typeof(Profile);
            if (profileTypes.Any(t => !ptype.IsAssignableFrom(t) && !t.IsAbstract && !t.IsClass))
            {
                throw new ArgumentOutOfRangeException(nameof(profileTypes), $"the type must be a subtype of Profile and not an abstract type");
            }
            builder.ServiceDescriptor.AddSingleton<IAutoMapperTypeDescription>(new AutoMapperTypeDescription(profileTypes));
            return builder;
        }

        public static IAutoMapperBuilder AddRalatedType(this IAutoMapperBuilder builder, Type ralatedType)
        {
            return AddRalatedTypes(builder, new[] { ralatedType });
        }

        public static IAutoMapperBuilder AddRalatedType<T>(this IAutoMapperBuilder builder)
        {
            return AddRalatedType(builder, typeof(T));
        }
        public static IAutoMapperBuilder AddRalatedTypes(this IAutoMapperBuilder builder, IEnumerable<Type> ralatedTypes)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (ralatedTypes == null)
            {
                throw new ArgumentNullException(nameof(ralatedTypes));
            }
            foreach (var t in ralatedTypes)
            {
                builder.ServiceDescriptor.TryAddTransient(t);
            }

            return builder;
        }

        #region CheckHelper
        private static void CheckNull(IAutoMapperBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
        }

        private static void CheckAssemblyNull(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
        }
        #endregion

        #region scan helper

        public static IEnumerable<Type> GetRelatedTypes(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => ScanAssembly(a, typeof(IValueResolver<,,>)))
                .Union(assemblies.SelectMany(a => ScanAssembly(a, typeof(IMemberValueResolver<,,,>))))
                .Union(assemblies.SelectMany(a => ScanAssembly(a, typeof(ITypeConverter<,>))))
                .Union(assemblies.SelectMany(a => ScanAssembly(a, typeof(IValueConverter<,>))))
                .Union(assemblies.SelectMany(a => ScanAssembly(a, typeof(IMappingAction<,>))))
                .ToList();
        }

        internal static IEnumerable<Type> ScanAssembly(this Assembly assembly, Type targetType)
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
        #endregion
    }
}
