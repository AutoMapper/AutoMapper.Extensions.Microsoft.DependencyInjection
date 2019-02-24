namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;

    public interface IConverter
    {
        TDestination Map1<TDestination>(object source);

        TDestination Map2<TSource, TDestination>(TSource source);

        TDestination Map3<TSource, TDestination>(TSource source, TDestination destination);

        object Map4(object source, Type sourceType, Type destinationType);

        object Map5(object source, object destination, Type sourceType, Type destinationType);
    }

    public interface IConverter1
    {
        TDestinationXXX Map1<TDestinationXXX>(object source);
    }

    public interface IConverter2
    {
        _TDestination Map2<TSource__, _TDestination>(TSource__ source);
    }

    public interface IConverter3
    {
        TDestin_ation Map3<TSo_urce, TDestin_ation>(TSo_urce source, TDestin_ation destination);
    }

    public interface IConverter4
    {
        object Map4(object sourceXXX, Type YYYsourceType, Type destinationTypeZZZ);
    }

    public interface IConverter5
    {
        object Map5(object sourCe, object destiNation, Type sourCeType, Type destinat444ionType);
    }
}
