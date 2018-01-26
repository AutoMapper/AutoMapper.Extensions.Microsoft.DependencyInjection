namespace AutoMapper
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="ConfigurationAssertMiddleware"/> that will call <see cref="IConfigurationProvider"/>.AssertConfigurationIsValid()
        /// to verify that AutoMapper's configuration is valid.
        /// </summary>
        /// <remarks>
        /// Since asserts are performed by exceptions, adding this middleware tends to return empty Internal Server Errors (500) unless
        /// this middleware is added after <see cref="IApplicationBuilder"/>.UseDeveloperExceptionPage()
        /// </remarks>
        public static void AssertAutoMapperConfigurationIsValid(this IApplicationBuilder app) => app.UseMiddleware<ConfigurationAssertMiddleware>();
    }

    internal sealed class ConfigurationAssertMiddleware
    {
        #region Internal State
        private readonly RequestDelegate nextStep;
        private readonly IConfigurationProvider configurationProvider;
        #endregion

        public ConfigurationAssertMiddleware(RequestDelegate nextStep, IConfigurationProvider configurationProvider)
        {
            // Initialize internal state
            this.nextStep = nextStep;
            this.configurationProvider = configurationProvider;
        }

        public Task Invoke(HttpContext httpContext)
        {
            configurationProvider.AssertConfigurationIsValid();

            return nextStep(httpContext);
        }
    }
}