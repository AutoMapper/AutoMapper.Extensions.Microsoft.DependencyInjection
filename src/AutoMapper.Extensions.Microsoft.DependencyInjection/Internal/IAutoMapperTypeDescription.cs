using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    public interface IAutoMapperTypeDescription
    {
        IEnumerable<Type> ProfileTypes { get; }
    }
}
