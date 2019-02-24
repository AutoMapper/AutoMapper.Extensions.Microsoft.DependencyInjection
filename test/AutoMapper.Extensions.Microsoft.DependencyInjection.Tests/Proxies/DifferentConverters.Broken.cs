namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;

    /// <summary> The interface is not suitable because it has no methods. </summary>
    public interface IConverter_Without_Anything
    {
    }

    /// <summary> The interface is not suitable because it has property. </summary>
    public interface IConverter_With_Property
    {
        int Age { get; }
    }

    /// <summary> The interface is not suitable because it has event. </summary>
    public interface IConverter_With_Event
    {
        event EventHandler Clicked;
    }

    /// <summary> The interface is not suitable because it has methods with the same signature. </summary>
    public interface IConverter_With_Duplicate_Method
    {
        TDestination Map<TDestination>(object source);

        TSomeResult DoIt<TSomeResult>(object dummy);
    }

    /// <summary> The interface is not suitable because it has a method with a signature that does not match any of the methods from <see cref = "IMapper" />. </summary>
    public interface IConverter_With_Unknown_Method
    {
        void DoSomething(int a, string b, DateTime c);
    }

    /// <summary> The interface is not suitable, because it is internal, proxies are built only for public interfaces. </summary>
    internal interface IConverter_Internal
    {
    }

    /// <summary> The interface does not fit, because it is not an interface, but a class. </summary>
    public class Converter_Type { }
}
