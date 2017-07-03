namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using Shouldly;
    using Xunit;

    public class TypeResolutionTests
    {
        private readonly IServiceProvider _provider;

        public TypeResolutionTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(mapper =>
            {
                mapper.AddTypeConverters(typeof(Source));
                mapper.AddValueResolvers(typeof(Source));
                mapper.AddProfiles(typeof(Source));
            });

            _provider = services.BuildServiceProvider();
        }

        [Fact]
        public void ShouldResolveConfiguration()
        {
            _provider.GetService<IConfigurationProvider>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldConfigureProfiles()
        {
            _provider.GetService<IConfigurationProvider>().GetAllTypeMaps().Length.ShouldBe(2);
        }

        [Fact]
        public void ShouldResolveMapper()
        {
            _provider.GetService<IMapper>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveValueResolver()
        {
            _provider.GetServices<SomeValueResolver>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveMemberValueResolver()
        {
            _provider.GetServices<SomeMemberValueResolver>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveTypeConverter()
        {
            _provider.GetService<SomeTypeConverter>().ShouldNotBeNull();
        }
    }
}