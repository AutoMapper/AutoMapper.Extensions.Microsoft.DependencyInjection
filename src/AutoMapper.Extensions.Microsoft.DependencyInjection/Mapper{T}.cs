using System;

namespace AutoMapper
{
    /// <summary>
    /// The implementation of <see cref="IMapper{T}"/>
    /// </summary>
    /// <typeparam name="T">The <see cref="IConfigurationProvider"/> implementation used to configure the mapper</typeparam>
    public class Mapper<T> : IMapper<T>
        where T : IConfigurationProvider
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapper{T}"/> class.
        /// </summary>
        /// <param name="configurationProvider">The configuration provider to be used to configure the mapper</param>
        /// <param name="provider">The service provider to resolve the services</param>
        public Mapper(T configurationProvider, IServiceProvider provider)
        {
            _mapper = new Mapper(configurationProvider, provider.GetService);
        }

        /// <inheritdoc />
        public IConfigurationProvider ConfigurationProvider => _mapper.ConfigurationProvider;

        /// <inheritdoc />
        public Func<Type, object> ServiceCtor => _mapper.ServiceCtor;

        /// <inheritdoc />
        public TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        /// <inheritdoc />
        public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> opts)
        {
            return _mapper.Map<TDestination>(source, opts);
        }

        /// <inheritdoc />
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        /// <inheritdoc />
        public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            return _mapper.Map(source, opts);
        }

        /// <inheritdoc />
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination);
        }

        /// <inheritdoc />
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            return _mapper.Map(source, destination, opts);
        }

        /// <inheritdoc />
        public object Map(object source, Type sourceType, Type destinationType)
        {
            return _mapper.Map(source, sourceType, destinationType);
        }

        /// <inheritdoc />
        public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
        {
            return _mapper.Map(source, sourceType, destinationType, opts);
        }

        /// <inheritdoc />
        public object Map(object source, object destination, Type sourceType, Type destinationType)
        {
            return _mapper.Map(source, destination, sourceType, destinationType);
        }

        /// <inheritdoc />
        public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
        {
            return _mapper.Map(source, destination, sourceType, destinationType, opts);
        }
    }
}