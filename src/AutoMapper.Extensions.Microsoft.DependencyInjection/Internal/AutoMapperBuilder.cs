using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Internal
{
    public class AutoMapperBuilder : IAutoMapperBuilder
    {
        public AutoMapperBuilder(IServiceCollection services)
        {
            ServiceDescriptor = services;
        }
        public IServiceCollection ServiceDescriptor { get; }
    }
}
