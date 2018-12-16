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

        IAutoMapperBuilder AddAssemblies(IEnumerable<Assembly> assemblies);
        IAutoMapperBuilder AddProfileTypes(IEnumerable<Type> profileTypes);

        IAutoMapperBuilder AddRalatedTypes(IEnumerable<Type> ralatedTypes);
    }
}
