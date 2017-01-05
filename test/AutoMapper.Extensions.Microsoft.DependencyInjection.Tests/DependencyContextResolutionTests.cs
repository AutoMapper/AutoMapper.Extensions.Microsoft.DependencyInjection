using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using System.Reflection;
    using global::Microsoft.Extensions.DependencyModel;
    using Shouldly;
    using Xunit;

    public class DependencyContextResolutionTests
    {
        private readonly IServiceProvider _provider;

        public DependencyContextResolutionTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(_ => { }, DependencyContext.Load(typeof(DependencyContextResolutionTests).GetTypeInfo().Assembly));
            _provider = services.BuildServiceProvider();
        }

        [Fact]
        public void ShouldResolveConfiguration()
        {
            _provider.GetService<IConfigurationProvider>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldNotThrowForNullEntryAssembly()
        {
            DependencyContext ignored = null;
            Action act = () => ignored = DependencyContext.Default;

            act.ShouldNotThrow();
            ignored.ShouldNotBeNull();
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