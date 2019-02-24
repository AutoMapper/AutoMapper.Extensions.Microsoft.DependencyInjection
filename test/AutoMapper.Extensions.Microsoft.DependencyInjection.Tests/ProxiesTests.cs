using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    public class ProxiesTests
    {
        [Fact]
        public void AddAutoMapperProxyShouldWork()
        {
            using (var container = new ServiceCollection()
                .AddTransient<Controller>()
                .AddAutoMapper()
                .AddAutoMapperProxy<IConverter>()
                .AddAutoMapperProxy<IConverter1>()
                .AddAutoMapperProxy<IConverter2>()
                .AddAutoMapperProxy<IConverter3>()
                .AddAutoMapperProxy<IConverter4>()
                .AddAutoMapperProxy<IConverter5>()
                .BuildServiceProvider())
            {
                using (var scope = container.CreateScope())
                {
                    Assert.Equal("OK!", scope.ServiceProvider.GetService<Controller>().DoIt());
                }
            }
        }

        [Theory]
        [InlineData(typeof(IConverter_Without_Anything))]
        [InlineData(typeof(IConverter_With_Property))]
        [InlineData(typeof(IConverter_With_Event))]
        [InlineData(typeof(IConverter_With_Duplicate_Method))]
        [InlineData(typeof(IConverter_With_Unknown_Method))]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        [InlineData(typeof(object))]
        [InlineData(typeof(IConverter_Internal))]
        [InlineData(typeof(Converter_Type))]
        public void AddAutoMapperProxyWithBadTypeShouldThrow(Type type)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var container = new ServiceCollection()
                    .AddAutoMapper()
                    .AddAutoMapperProxy(type)
                    .BuildServiceProvider())
                {
                }
            });
        }
    }
}
