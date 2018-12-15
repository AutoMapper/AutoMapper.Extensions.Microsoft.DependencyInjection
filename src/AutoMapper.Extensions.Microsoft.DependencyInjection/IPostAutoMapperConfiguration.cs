using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection
{
    public interface IPostAutoMapperConfiguration
    {
        void Configuration(IServiceProvider provider, IMapperConfigurationExpression configurationExpression);
    }
}
