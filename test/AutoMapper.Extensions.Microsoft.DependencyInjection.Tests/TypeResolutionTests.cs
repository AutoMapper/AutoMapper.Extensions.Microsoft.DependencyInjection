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
            services.AddSingleton<ISomeService2, TrimStringService>();
            services.AddAutoMapper(typeof(Source));
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
            var typeMaps = _provider.GetService<IConfigurationProvider>().GetAllTypeMaps();

            // Assert the correct number of Type Maps have been configured
            typeMaps.Length.ShouldBe(4);

            // Assert the correct Type Maps have been configured
            typeMaps.ShouldContain(tm => tm.SourceType == typeof(Source) && tm.DestinationType == typeof(Dest));
            typeMaps.ShouldContain(tm => tm.SourceType == typeof(Source2) && tm.DestinationType == typeof(Dest2));
            typeMaps.ShouldContain(tm => tm.SourceType == typeof(Source4) && tm.DestinationType == typeof(Dest4));
            typeMaps.ShouldContain(tm => tm.SourceType == typeof(Source3) && tm.DestinationType == typeof(Dest3));
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

        [Fact]
        public void ShouldResolveValueConverter()
        {
            _provider.GetService<FooValueConverter>().ShouldNotBeNull();
        }
    }
}