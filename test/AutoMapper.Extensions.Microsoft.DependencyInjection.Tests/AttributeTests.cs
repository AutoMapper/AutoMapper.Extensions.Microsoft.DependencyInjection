﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    public class AttributeTests
    {
        [Fact]
        public void Should_not_register_static_instance_when_configured()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(typeof(Source3));

            var serviceProvider = services.BuildServiceProvider();

            var mapper = serviceProvider.GetService<IMapper>();

            var source = new Source3 {Value = 3};

            var dest = mapper.Map<Dest3>(source);

            dest.Value.ShouldBe(source.Value);
        }
    }
}