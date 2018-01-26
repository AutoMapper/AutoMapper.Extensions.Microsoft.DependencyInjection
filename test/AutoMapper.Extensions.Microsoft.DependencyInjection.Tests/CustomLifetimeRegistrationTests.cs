using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    public class CustomLifetimeRegistrationTests
    {
        private readonly ServiceCollection _services;
        private static readonly Func<TypeInfo, ServiceLifetime> ServiceLifetimeSelector = info => ServiceLifetime.Singleton;

        public CustomLifetimeRegistrationTests()
        {
            _services = new ServiceCollection();

        }

        [Fact]
        public void CanSpecifyLifetimeWithDefaultRegistration()
        {
            _services.AddAutoMapper(ServiceLifetimeSelector, ServiceLifetime.Singleton);
            CheckMapperRegistration(ServiceLifetime.Singleton);
            CheckServiceRegistration(ServiceLifetime.Singleton);
        }

        [Fact]
        public void CanSpecifyLifetimeWithTypeMarker()
        {
            _services.AddAutoMapper(null, ServiceLifetimeSelector, ServiceLifetime.Singleton, typeof(CustomTypeConverter));
            CheckMapperRegistration(ServiceLifetime.Singleton);
            CheckServiceRegistration(ServiceLifetime.Singleton);
        }

        [Fact]
        public void CanSpecifyLifetimeWithAssemblyParams()
        {
            _services.AddAutoMapper(null, ServiceLifetimeSelector, ServiceLifetime.Singleton, typeof(CustomTypeConverter).Assembly);
            CheckMapperRegistration(ServiceLifetime.Singleton);
            CheckServiceRegistration(ServiceLifetime.Singleton);
        }

        [Fact]
        public void CanSpecifyLifetimeWithAssemblyList()
        {
            _services.AddAutoMapper(new[] { typeof(CustomTypeConverter).Assembly }, ServiceLifetimeSelector, ServiceLifetime.Singleton);
            CheckMapperRegistration(ServiceLifetime.Singleton);
            CheckServiceRegistration(ServiceLifetime.Singleton);
        }

        [Fact]
        public void CanSpecifyLifetimeWithCustomActionAndAssemblyList()
        {
            _services.AddAutoMapper(null, new[] { typeof(CustomTypeConverter).Assembly }, ServiceLifetimeSelector, ServiceLifetime.Singleton);
            CheckMapperRegistration(ServiceLifetime.Singleton);
            CheckServiceRegistration(ServiceLifetime.Singleton);
        }


        class CustomTypeConverter : ITypeConverter<byte, char>
        {
            public char Convert(byte source, char destination, ResolutionContext context)
            {
                return (char) source;
            }
        }

        private void CheckServiceRegistration(ServiceLifetime serviceLifetime)
        {
            _services.ShouldContain(d => d.ImplementationType == typeof(CustomTypeConverter) && d.Lifetime == serviceLifetime);
        }

        private void CheckMapperRegistration(ServiceLifetime mapperLifetime)
        {
            _services.ShouldContain(d => d.ServiceType == typeof(IMapper) && d.Lifetime == mapperLifetime, "Mapper registration is not correct.");
        }
    }
}

