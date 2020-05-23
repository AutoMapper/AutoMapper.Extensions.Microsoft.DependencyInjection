using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    public class ValidateTests
    {
        [Fact]
        public void ShouldFailAssertForInvalidProfile()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddAutoMapper(true, typeof(InvalidProfile));

            // Assert
            var provider = serviceCollection.BuildServiceProvider();
            Assert.Throws<AutoMapperConfigurationException>(() => provider.GetRequiredService<IMapper>());
        }

        class InvalidProfile : Profile
        {
            public InvalidProfile()
            {
                // PropertyTwo is unmapped, profile is invalid
                CreateMap<Source, Destination>();
            }
        }

        class Source
        {
            public string PropertyOne { get; set; }
        }

        class Destination
        {
            public string PropertyTwo { get; set; }
        }
    }
}
