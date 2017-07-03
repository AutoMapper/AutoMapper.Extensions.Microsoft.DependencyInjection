namespace AutoMapper
{
    using System;

    internal interface IMapperConfigurationExpressionAction
    {
        Action<IMapperConfigurationExpression> Action { get; }
    }
}