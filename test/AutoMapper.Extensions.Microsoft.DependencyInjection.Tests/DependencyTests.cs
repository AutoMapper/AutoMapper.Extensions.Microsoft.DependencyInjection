namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using Shouldly;
    using Xunit;

    public class DependencyTests
    {
        private readonly IServiceProvider _provider;

        public DependencyTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<ISomeService>(sp => new FooService(5));
            services.AddAutoMapper(typeof(Source), typeof(Profile));
            _provider = services.BuildServiceProvider();
        }

        [Fact]
        public void ShouldResolveWithDependency()
        {
            var mapper = _provider.GetService<IMapper>();
            var dest = mapper.Map<Source2, Dest2>(new Source2());

            dest.ResolvedValue.ShouldBe(5);
        }

        [Fact]
        public void ShouldResolveTypedWithDependency()
        {
            var dest2 = _provider.GetService<IMapper<ConfigurationWith2>>().Map<Source2, Dest2>(new Source2());
            dest2.ResolvedValue.ShouldBe(2);
        }
    }
}