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

        private bool _useStaticInitialization;

        IServiceCollection IMapperBuilder.Services => _services;

        IList<Action<IMapperConfigurationExpression>> IMapperBuilder.ConfigurationExpressionActions => _configActions;

        bool IMapperBuilder.UseStaticInitialization => _useStaticInitialization;

        public MapperBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public MapperBuilder UseStaticInitialization(bool useStatic = true)
        {
            _useStaticInitialization = useStatic;
            return this;
        }

        public MapperBuilder UseConfiguration(Action<IMapperConfigurationExpression> configAction)
        {
            _configActions.Add(configAction);
            return this;
        }
    }
}