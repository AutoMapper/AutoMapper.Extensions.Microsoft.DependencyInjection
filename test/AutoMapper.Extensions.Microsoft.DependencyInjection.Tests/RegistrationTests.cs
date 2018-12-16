using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    [Collection(nameof(RegistrationTests))]
    public class RegistrationTests
    {
        [Fact]
        public void Should_not_register_static_instance_when_configured()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper();

            var serviceProvider = services.BuildServiceProvider();

            var config = serviceProvider.GetService<IConfigurationProvider>();
            config.ShouldNotBeNull();

            try
            {
                config.ShouldNotBeSameAs(Mapper.Configuration);
            }
            catch (InvalidOperationException)
            {
                // Success if the mapper has not been initialized anyway
            }
        }

        [Fact]
        public void Should_allow_multiple_AddAutoMapper_when_configured()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper()
                .AddAssembly(typeof(Dest).Assembly);

            services.AddAutoMapper()
                .AddProfileType<Profile2>();

            var serviceProvider = services.BuildServiceProvider();

            var config = serviceProvider.GetService<IConfigurationProvider>();
            config.ShouldNotBeNull();
            var typeProvider = serviceProvider.GetRequiredService<IAutoMapperConfigurationProvider>();
            var profileTypes = typeProvider.GetMapProfileTypes();
            Assert.Equal(2, profileTypes.Count());
            try
            {
                config.ShouldNotBeSameAs(Mapper.Configuration);
            }
            catch (InvalidOperationException)
            {
                // Success if the mapper has not been initialized anyway
            }
        }
    }
}