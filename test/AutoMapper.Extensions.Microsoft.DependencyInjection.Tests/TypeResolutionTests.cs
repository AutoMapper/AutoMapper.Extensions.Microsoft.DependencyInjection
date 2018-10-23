using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using Shouldly;
    using Xunit;

    public class TypeResolutionTests
    {
        private readonly IServiceProvider _provider;

        public TypeResolutionTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(typeof(Source));
            services.AddTransient<ISomeService>(sp => new FooService(5));
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
            _provider.GetService<IConfigurationProvider>().GetAllTypeMaps().Length.ShouldBe(3);
        }

        [Fact]
        public void ShouldResolveMapper()
        {
            _provider.GetService<IMapper>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveMappingAction()
        {
            _provider.GetService<FooMappingAction>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveValueResolver()
        {
            _provider.GetService<FooValueResolver>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveMemberValueResolver()
        {
            _provider.GetService<FooMemberValueResolver>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveTypeConverter()
        {
            _provider.GetService<FooTypeConverter>().ShouldNotBeNull();
        }
    }
}