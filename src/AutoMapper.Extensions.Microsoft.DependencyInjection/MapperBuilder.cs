namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;

    public class MapperBuilder : IMapperBuilder
    {
        private readonly IServiceCollection _services;

        private readonly IList<Action<IMapperConfigurationExpression>> _configActions =
            new List<Action<IMapperConfigurationExpression>>();

        IServiceCollection IMapperBuilder.Services => _services;

        IList<Action<IMapperConfigurationExpression>> IMapperBuilder.ConfigActions => _configActions;

        public MapperBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public MapperBuilder UseConfiguration(Action<IMapperConfigurationExpression> configAction)
        {
            _configActions.Add(configAction);
            return this;
        }
    }
}