namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using global::Microsoft.Extensions.DependencyInjection;
    using Shouldly;
    using Xunit;

    public class StaticInitializationTests
    {
        public StaticInitializationTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(builder =>
            {
                builder.UseStaticInitialization();
                builder.AddProfiles(typeof(Source));
            });
        }

        [Fact]
        public void ShouldInitializeStatically()
        {
            Mapper.Configuration.GetAllTypeMaps().Length.ShouldBe(2);
        }
    }
}