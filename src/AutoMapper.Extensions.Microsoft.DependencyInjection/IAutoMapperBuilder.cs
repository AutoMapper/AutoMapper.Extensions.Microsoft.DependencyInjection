using AutoMapper.Extensions.Microsoft.DependencyInjection.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection
{
    public interface IAutoMapperBuilder
    {
        IServiceCollection ServiceDescriptor { get; }

    }

    public static class IAutoMapperBuilderExtensions
    {
        public static IAutoMapperBuilder AddAssembly(this IAutoMapperBuilder builder,Assembly assembly)
        {
            CheckNull(builder);
            CheckAssemblyNull(assembly);
            builder.ServiceDescriptor.AddSingleton<IAutoMapperAssemblyDescription>(new AutoMapperAssemblyDescription(assembly));
            return builder;
        }

        public static IAutoMapperBuilder AddAssemblies(this IAutoMapperBuilder builder,params Assembly[] assemblies)
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
            foreach(var assembly in assemblies)
            {
                AddAssembly(builder, assembly);
            }
            return builder;
        }


        public static IAutoMapperBuilder AddProfileType(this IAutoMapperBuilder builder,Type profileType)
        {
            if (profileType == null)
            {
                throw new ArgumentNullException(nameof(profileType));
            }

            var ptype = typeof(Profile);
            if (!ptype.IsAssignableFrom(profileType) && !profileType.IsAbstract)
            {
                throw new ArgumentOutOfRangeException(nameof(profileType), $"the type must be a subtype of Profile and not an abstract type");
            }

            // 

            return builder;
        }

        public static IAutoMapperBuilder AddProfileType<T>(this IAutoMapperBuilder builder) where T:Profile
        {
            return AddProfileType(builder, typeof(T));
        }

        public static IAutoMapperBuilder AddProfileTypes(this IAutoMapperBuilder builder,IEnumerable<Type> profileType)
        {
            foreach(var t in profileType)
            {
                if(t == null)
                {
                    continue;
                }
                AddProfileType(builder, t);
            }
            return builder;
        }

        #region CheckHelper
        private static void CheckNull(IAutoMapperBuilder builder)
        {
            if(builder == null)
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
    }
}
