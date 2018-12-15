using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    public class AutoMapperAssemblyDescription : IAutoMapperAssemblyDescription
    {
        private readonly IEnumerable<Assembly> _assemblies ;
        public AutoMapperAssemblyDescription(Assembly assembly):this(new[] { assembly })
        {
            assembly = assembly ?? throw new ArgumentNullException();
        }

        public AutoMapperAssemblyDescription(IEnumerable<Assembly> assemblies)
        {
            Assemblies = assemblies;
        }

        public IEnumerable<Assembly> Assemblies { get; }
    }
}
