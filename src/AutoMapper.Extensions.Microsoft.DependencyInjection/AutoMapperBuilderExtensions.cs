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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            builder.AddAssemblies(new[] { assembly });

            return builder;
        }

        public static IAutoMapperBuilder AddAssemblies(this IAutoMapperBuilder builder, params Assembly[] assemblies)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddAssemblies((IEnumerable<Assembly>)assemblies);
            return builder;
        }



        public static IAutoMapperBuilder AddProfileType(this IAutoMapperBuilder builder, Type profileType)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddProfileTypes(new[] { profileType });

            return builder;
        }

        public static IAutoMapperBuilder AddProfileType<T>(this IAutoMapperBuilder builder) where T : Profile
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddProfileType(typeof(T));
        }


        public static IAutoMapperBuilder AddRalatedType(this IAutoMapperBuilder builder, Type ralatedType)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddRalatedTypes(new[] { ralatedType });
        }

        public static IAutoMapperBuilder AddRalatedType<T>(this IAutoMapperBuilder builder)
        {
            return builder.AddRalatedType(typeof(T));
        }
    }
}
