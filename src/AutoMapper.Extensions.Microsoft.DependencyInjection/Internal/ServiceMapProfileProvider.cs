using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    public class ServiceMapProfileProvider : IAutoMapConfigurationProvider
    {
        private readonly IEnumerable<IAutoMapperAssemblyDescription> _assemblyDescriptions;
        private readonly IEnumerable<IAutoMapperTypeDescription> _typeDescriptions;

        public ServiceMapProfileProvider(IEnumerable<IAutoMapperAssemblyDescription> assemblyDescriptions,
            IEnumerable<IAutoMapperTypeDescription> typeDescriptions)
        {
            _assemblyDescriptions = assemblyDescriptions;
            _typeDescriptions = typeDescriptions;
        }

        private IEnumerable<Assembly> Assemblies => _assemblyDescriptions.SelectMany(d => d.Assemblies).Distinct();

        public IEnumerable<Type> GetMapProfilerTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> GetRelatedTypes()
        {
            throw new NotImplementedException();
        }
    }
}
