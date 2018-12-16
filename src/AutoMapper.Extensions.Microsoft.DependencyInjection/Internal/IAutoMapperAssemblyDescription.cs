using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection
{
    /// <summary>
    /// represents some assembly that AutoMapper should scan
    /// </summary>
    public interface IAutoMapperAssemblyDescription
    {
        IEnumerable<Assembly> Assemblies { get; }
    }
}
