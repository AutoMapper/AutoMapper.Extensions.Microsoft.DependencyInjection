using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using System.Reflection;
    using global::Microsoft.Extensions.DependencyModel;
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
        public void ShouldRegisterConfigurationAsSingleton()
        {
            using (var scope = _provider.CreateScope())
            {
                var first = _provider.GetService<IConfigurationProvider>();
                var second = _provider.GetService<IConfigurationProvider>();
                var third = scope.ServiceProvider.GetService<IConfigurationProvider>();

                first.ShouldBeSameAs(second);
                first.ShouldBeSameAs(third);
            }
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
        public void ShouldRegisterMapperAsScoped()
        {
            using (var outerScope = _provider.CreateScope())
            using (var innerScope = outerScope.ServiceProvider.CreateScope())
            {
                var first = outerScope.ServiceProvider.GetService<IMapper>();
                var second = outerScope.ServiceProvider.GetService<IMapper>();
                var third = innerScope.ServiceProvider.GetService<IMapper>();

                first.ShouldBeSameAs(second);
                first.ShouldNotBeSameAs(third);
            }
        }
    }
}