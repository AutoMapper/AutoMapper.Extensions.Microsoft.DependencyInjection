namespace AutoMapper
{
    using System;

    internal class MapperConfigurationExpressionAction : IMapperConfigurationExpressionAction
    {
        public Action<IMapperConfigurationExpression> Action { get; }

        public MapperConfigurationExpressionAction(Action<IMapperConfigurationExpression> action)
        {
            Action = action;
        }
    }
}