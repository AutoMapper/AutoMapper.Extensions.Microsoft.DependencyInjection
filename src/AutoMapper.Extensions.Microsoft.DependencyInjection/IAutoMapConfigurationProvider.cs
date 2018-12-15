using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection
{
    public interface IAutoMapConfigurationProvider
    {
        IEnumerable<Type> GetMapProfilerTypes();

        IEnumerable<Type> GetRelatedTypes();
    }
}
