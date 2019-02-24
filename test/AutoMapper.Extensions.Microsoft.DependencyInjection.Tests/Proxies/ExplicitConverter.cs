namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;

    /// <summary>
    /// Just an example of the class being created in runtime.
    /// </summary>
    internal sealed class ExplicitConverter : IConverter
    {
        private readonly IMapper _mapper;

        public ExplicitConverter(IMapper mapper) => _mapper = mapper;

        public TDestination Map1<TDestination>(object source) => _mapper.Map<TDestination>(source);

        public TDestination Map2<TSource, TDestination>(TSource source) => _mapper.Map<TSource, TDestination>(source);

        public TDestination Map3<TSource, TDestination>(TSource source, TDestination destination) => _mapper.Map<TSource, TDestination>(source, destination);

        public object Map4(object source, Type sourceType, Type destinationType) => _mapper.Map(source, sourceType, destinationType);

        public object Map5(object source, object destination, Type sourceType, Type destinationType) => _mapper.Map(source, destination, sourceType, destinationType);
    }
}
