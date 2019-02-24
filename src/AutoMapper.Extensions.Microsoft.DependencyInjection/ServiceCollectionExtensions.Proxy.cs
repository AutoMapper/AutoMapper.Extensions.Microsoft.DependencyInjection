namespace AutoMapper
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// After calling <see cref="AddAutoMapperProxy"/> you can resolve an <paramref name="proxiedInterface"/> instance from a scoped service provider, or as a dependency.
        /// </summary>
        /// <param name="services"> An instance of <see cref="IServiceCollection"/>. </param>
        /// <param name="proxiedInterface"> The interface for which you want to create a proxy. </param>
        /// <returns> Reference to <paramref name="services"/>. </returns>
        public static IServiceCollection AddAutoMapperProxy(this IServiceCollection services, Type proxiedInterface)
        {
            if (proxiedInterface == null)
                throw new ArgumentNullException(nameof(proxiedInterface));

            var proxyInfo = ProxyInfo.BuildFrom(proxiedInterface);

            return services.AddScoped(proxiedInterface, provider =>
            {
                var implType = ProxyGenerator.Generate(proxyInfo);
                return Activator.CreateInstance(implType, provider.GetService<IMapper>());
            });
        }

        /// <summary>
        /// After calling <see cref="AddAutoMapperProxy"/> you can resolve an <typeparamref name="TProxyInterface"/> instance from a scoped service provider, or as a dependency.
        /// </summary>
        /// <typeparam name="TProxyInterface"> The interface for which you want to create a proxy. </typeparam>
        /// <param name="services"> An instance of <see cref="IServiceCollection"/>. </param>
        /// <returns> Reference to <paramref name="services"/>. </returns>
        public static IServiceCollection AddAutoMapperProxy<TProxyInterface>(this IServiceCollection services) => services.AddAutoMapperProxy(typeof(TProxyInterface));
    }
}
