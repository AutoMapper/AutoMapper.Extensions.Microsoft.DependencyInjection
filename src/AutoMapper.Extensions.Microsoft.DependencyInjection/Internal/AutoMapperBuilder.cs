using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static AutoMapper.Extensions.Microsoft.DependencyInjection.Internal.AssemblyScanHelper;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    public class AutoMapperBuilder : IAutoMapperBuilder
    {
        public AutoMapperBuilder(IServiceCollection services)
        {
            ServiceDescriptor = services;
        }
        public IServiceCollection ServiceDescriptor { get; }

        public IAutoMapperBuilder AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }
            this.ServiceDescriptor.AddSingleton<IAutoMapperAssemblyDescription>(new AutoMapperAssemblyDescription(assemblies));

            var relatedTypes = GetRelatedTypes(assemblies);
            foreach (var t in relatedTypes)
            {
                this.ServiceDescriptor.TryAddTransient(t);
            }
            return this;
        }


        public IAutoMapperBuilder AddProfileTypes(IEnumerable<Type> profileTypes)
        {
            if (profileTypes == null)
            {
                throw new ArgumentNullException(nameof(profileTypes));
            }
            var ptype = typeof(Profile);
            if (profileTypes.Any(t => !ptype.IsAssignableFrom(t) && !t.IsAbstract && !t.IsClass))
            {
                throw new ArgumentOutOfRangeException(nameof(profileTypes), $"the type must be a subtype of Profile and not an abstract type");
            }
            this.ServiceDescriptor.AddSingleton<IAutoMapperTypeDescription>(new AutoMapperTypeDescription(profileTypes));
            return this;
        }


        public IAutoMapperBuilder AddRalatedTypes(IEnumerable<Type> ralatedTypes)
        {
            if (ralatedTypes == null)
            {
                throw new ArgumentNullException(nameof(ralatedTypes));
            }
            foreach (var t in ralatedTypes)
            {
                this.ServiceDescriptor.TryAddTransient(t);
            }

            return this;
        }
    }
}
