using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    public class AutoMapperConfigurationProvider : IAutoMapperConfigurationProvider
    {
        private readonly IEnumerable<IAutoMapperAssemblyDescription> _assemblyDescriptions;
        private readonly IEnumerable<IAutoMapperTypeDescription> _typeDescriptions;

        public AutoMapperConfigurationProvider(IEnumerable<IAutoMapperAssemblyDescription> assemblyDescriptions,
            IEnumerable<IAutoMapperTypeDescription> typeDescriptions)
        {
            _assemblyDescriptions = assemblyDescriptions;
            _typeDescriptions = typeDescriptions;
        }

        private IEnumerable<Assembly> Assemblies
        {
            get
            {
                if(_assemblyDescriptions.Count() > 0)
                {
                    return _assemblyDescriptions.SelectMany(d => d.Assemblies).Distinct();
                }
                else
                {
                    return AppDomain.CurrentDomain.GetAssemblies();
                }
            }
        }

        public IEnumerable<Type> GetMapProfileTypes()
        {
            return Assemblies.SelectMany(a => a.ScanAssembly(typeof(Profile)))
                .Union(_typeDescriptions.SelectMany(x=>x.ProfileTypes));
        }

        


        //private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        //    => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        //private static bool IsGenericType(this Type type, Type genericType)
        //    => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
