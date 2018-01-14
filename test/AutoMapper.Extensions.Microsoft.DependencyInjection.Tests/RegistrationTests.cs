using System;
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
            ServiceCollectionExtensions.UseStaticRegistration = false;

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
        public void Register_with_profile_via_generic()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper<Profile1>();
            IServiceProvider sp = services.BuildServiceProvider();
            var mapper = sp.GetService<IMapper>();
            Assert.NotNull(mapper.Map<Dest>(new Source()));
        }     
    }
}