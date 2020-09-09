using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    public class MultipleRegistrationTests
    {
        [Fact]
        public void Can_register_multiple_times()
        {
            static void registration(IServiceCollection services)
                => services.AddAutoMapper(cfg => { });

            AutoMapperShouldBeRegistrableMultipleTimesUsing(registration);
        }

        [Fact]
        public void Can_register_multiple_times_using_service_provider()
        {
            static void registration(IServiceCollection services)
                => services.AddAutoMapper((cfg, sp) => { });

            AutoMapperShouldBeRegistrableMultipleTimesUsing(registration);
        }

        private void AutoMapperShouldBeRegistrableMultipleTimesUsing(Action<IServiceCollection> registration)
        {
            var services = new ServiceCollection();

            registration(services);
            registration(services);
            registration(services);

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<IMapper>().ShouldNotBeNull();
        }

        [Fact]
        public void Can_register_assembly_multiple_times()
        {
            var services = new ServiceCollection();

            services.AddAutoMapper(typeof(MultipleRegistrationTests));
            services.AddAutoMapper(typeof(MultipleRegistrationTests));
            services.AddAutoMapper(typeof(MultipleRegistrationTests));
            services.AddTransient<ISomeService, MutableService>();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<IMapper>().ShouldNotBeNull();
            serviceProvider.GetService<DependencyValueConverter>().ShouldNotBeNull();
            serviceProvider.GetServices<DependencyValueConverter>().Count().ShouldBe(1);
        }
    }
}