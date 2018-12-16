using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection
{
    public interface IAutoMapperConfigurationProvider
    {
        IEnumerable<Type> GetMapProfileTypes();

    }
}
