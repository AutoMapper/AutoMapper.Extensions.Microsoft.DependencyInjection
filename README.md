# AutoMapper extensions for Microsoft.Extensions.DependencyInjection

Scans assemblies and:

1. adds profiles to mapping configuration
2. adds value resolvers, member value resolvers, type converters to the container.

To use, with an `IServiceCollection` instance and one or more assemblies:

```c#
services.AddAutoMapper(assembly1, assembly2 /*, ...*/);
```

or marker types:

```c#
services.AddAutoMapper(type1, type2 /*, ...*/);
```

This registers AutoMapper:

- As a singleton for the `MapperConfiguration`
- As a scoped instance for `IMapper`
- `ITypeConverter` instances as transient
- `IValueConverter` instances as transient
- `IValueResolver` instances as transient
- `IMemberValueResolver` instances as transient
- `IMappingAction` instances as transient

Mapping configuration is static as it is the root object that can create an `IMapper`.

Mapper instances are registered as scoped as they are intended to be used within the lifetime of a request. Since a `Mapper` instance can internally create other instances during mapping, it cannot be registered statically.

### Mapper.Map usage

To map at runtime, you'll first need a scope. In an ASP.NET Core application, each request already has a scope, so you can depend on `IMapper` directly:

```c#
public class EmployeesController {
	private readonly IMapper _mapper;

	public EmployeesController(IMapper mapper)
		=> _mapper = mapper;

	// use _mapper.Map to map
}
```

You cannot use the static `Mapper` class to map, as this does not play nicely with dependency injection.

### ProjectTo usage

Starting with 8.0 you can use `IMapper.ProjectTo`. The old `ProjectTo` is an extension method and does not have dependency injection available. Pass an `IConfigurationProvider` instance directly:

```c#
var orders = await dbContext.Orders
                       .ProjectTo<OrderDto>(_configurationProvider)
					   .ToListAsync();
```

Or you can use an `IMapper` instance:

```c#
var orders = await dbContext.Orders
                       .ProjectTo<OrderDto>(_mapper)
					   .ToListAsync();
```

If you use `ProjectTo` without passing in the configuration instance, AutoMapper falls back to the uninitialized static instance, and you will see a runtime exception.

### Configuration Validation
Don't use the static `Mapper.Configuration.AssertConfigurationIsValid()`, it just won't work. Instead you can let the DI framework inject an instance of `IMapper` into your `Configure()` method. You can then use its ConfigurationProvider property to call `AssertConfigurationIsValid()`.

``` diff
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(/* ... */);
    }

-   public void Configure(IApplicationBuilder app, IHostingEnvironment env)
+   public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper autoMapper)
    {
+        autoMapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
```

You can also inject just the `IConfigurationProvider`, but beware that an interface with the same name exists in the Microsoft.Extensions.Configuration namespace so use a fully qualified name.

``` diff
-public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper autoMapper)
+public void Configure(IApplicationBuilder app, IHostingEnvironment env, AutoMapper.IConfigurationProvider autoMapper)
{
-    autoMapper.ConfigurationProvider.AssertConfigurationIsValid();
+    autoMapper.AssertConfigurationIsValid();
}
```

### AddAutoMapperProxy usage

The `AddAutoMapper` method allows you to register an `IMapper` instance and auxiliary entities in a `IServiceCollection` container. Next, you can inject the `IMapper` dependency inside the application.
This is suitable in most cases if full compliance with the principle of dependency inversion is not required. However, in large applications that have a layered architecture, this can lead to negative
consequences, since the introduction of dependency on the `IMapper` violates one of DI principles:

> Abstractions should not depend on details. Details should depend on abstractions.

In this case, the `IMapper` interface acts as an implementation, not an abstraction, since the code using it makes a dependency on a specific library (and version) that implements the function of mapping objects.
To avoid this, the developer can define his own dependency interface and accept it into the constructor instead of the `IMapper` interface:
```c#
public interface IConverter
{
    TDestination Convert<TDestination>(object source);
}

public class EmployeesController {
	private readonly IConverter _converter;

	public EmployeesController(IConverter converter) // no more direct dependency from AutoMapper package
		=> _converter = converter;

	// use _converter.Convert
}
```

And register proxy in composition root:
```c#
services.AddAutoMapperProxy<IConverter>();
//or
services.AddAutoMapperProxy(typeof(IConverter));
```

In this case, the application libraries do not contain an external dependency on the AutoMapper package. The only place that sets the dependency is the composition root. If necessary, the specific
implementation of the mapping mechanism can be easily changed without the need to edit the code of the entire application.

Under the hood, the `AddAutoMapperProxy` method builds a special proxy class in runtime and adds scoped dependency from it into `IServiceCollection`. Methods of this class just delegate calls
to `IMapper`. Matching is done by the methods signature, the name of method can be arbitrary. It should be noted that exactly the same class could be created by the developer at compile time to
solve the above problem. In this regard, the `AddAutoMapperProxy` method frees the developer from this boring job.

Supported proxied `IMapper`'s methods:
```c#
TDestination Map<TDestination>(object source);
TDestination Map<TSource, TDestination>(TSource source);
TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
object Map(object source, Type sourceType, Type destinationType);
object Map(object source, object destination, Type sourceType, Type destinationType);
```