using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using System.Reflection;
    using Shouldly;
    using Xunit;

    public class AssemblyResolutionTests
    {
        private static readonly IServiceProvider _provider;

        static AssemblyResolutionTests()
        {
            _provider = BuildServiceProvider();    
        }

        private static ServiceProvider BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ISomeService2, TrimStringService>();
            services.AddAutoMapper(typeof(Source).GetTypeInfo().Assembly);
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
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
        public void CanRegisterTwiceWithoutProblems()
        {
            new Action(() => BuildServiceProvider()).ShouldNotThrow();
        }
    }
}