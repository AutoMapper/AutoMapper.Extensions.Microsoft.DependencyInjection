namespace AutoMapper
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;

    public interface IMapperBuilder
    {
        IServiceCollection Services { get; }

        IList<Action<IMapperConfigurationExpression>> ConfigActions { get; }
    }
}