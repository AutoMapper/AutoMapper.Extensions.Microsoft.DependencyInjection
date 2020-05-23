using Microsoft.Extensions.DependencyInjection;
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
    }
}
