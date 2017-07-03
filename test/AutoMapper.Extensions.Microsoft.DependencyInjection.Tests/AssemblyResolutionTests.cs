namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using global::Microsoft.Extensions.DependencyInjection;
    using Shouldly;
    using Xunit;
    using System.Reflection;

    public class AssemblyResolutionTests
    {
        private readonly IServiceProvider _provider;

        public AssemblyResolutionTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(mapper =>
            {
                mapper.AddTypeConverters(typeof(Source).GetTypeInfo().Assembly);
                mapper.AddValueResolvers(typeof(Source).GetTypeInfo().Assembly);
                mapper.AddProfiles(typeof(Source).GetTypeInfo().Assembly);
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