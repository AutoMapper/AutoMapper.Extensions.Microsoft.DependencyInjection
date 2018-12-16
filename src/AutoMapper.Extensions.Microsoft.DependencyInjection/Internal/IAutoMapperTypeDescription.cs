using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    /// <summary>
    /// represents some <see cref="Profile"/> type that AutoMapper should care
    /// </summary>
    public interface IAutoMapperTypeDescription
    {
        IEnumerable<Type> ProfileTypes { get; }
    }
}
