using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection
{
    /// <summary>
    /// represents an <see cref="Action{T1, T2}"/> that AutoMapper should care
    /// </summary>
    public interface IPostAutoMapperConfiguration
    {
        void Configuration(IServiceProvider provider, IMapperConfigurationExpression configurationExpression);
    }
}
