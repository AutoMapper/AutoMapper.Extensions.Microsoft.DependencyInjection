using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using System.Reflection;
    using Shouldly;
    using Xunit;

    public class AssemblyResolutionTests
    {
        private readonly IServiceProvider _provider;

        public AssemblyResolutionTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(typeof(Source).GetTypeInfo().Assembly);
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
        public void ShouldInitializeStatically()
        {
            _provider.GetService<IConfigurationProvider>().ShouldBeSameAs(Mapper.Configuration);
        }
    }
}