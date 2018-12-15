using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    public class PostAutoMapperConfiguration : IPostAutoMapperConfiguration
    {
        private readonly Action<IServiceProvider, IMapperConfigurationExpression> _action;

        public PostAutoMapperConfiguration(Action<IServiceProvider,IMapperConfigurationExpression> action)
        {
            _action = action;
        }
        public void Configuration(IServiceProvider provider, IMapperConfigurationExpression configurationExpression)
        {
            _action?.Invoke(provider, configurationExpression);
        }
    }
}
