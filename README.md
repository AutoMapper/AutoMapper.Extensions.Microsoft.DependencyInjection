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
- `IValueResolver` instances as transient
- `IMemberValueResolver` instances as transient
- `IMappingAction` instances as transient

Mapping configuration is static as it is the root object that can create an `IMapper`.

Mapper instances are registered as scoped as they are intented to be used within the lifetime of a request. Since a `Mapper` instance can internally create other instances during mapping, it cannot be registered statically.

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

`ProjectTo` is an extension method and does not have dependency injection available. Pass an `IConfigurationProvider` instance directly:

```c#
var orders = await dbContext.Orders
                       .ProjectTo<OrderDto>(_configurationProvider)
					   .ToListAsync();
```

Or you can use an `IMapper` instance:

```c#
var orders = await dbContext.Orders
                       .ProjectTo<OrderDto>(_mapper.Configuration)
					   .ToListAsync();
```

If you use `ProjectTo` without passing in the configuration instance, AutoMapper falls back to the uninitialized static instance, and you will see a runtime exception.

### Configuration Validation
Don't use the static `Mapper.Configuration.AssertConfigurationIsValid()`, it just won't work. Instead you can let the DI framework inject an instance of `IMapper` into your `Configure()` method. You can then use its Configuration property to call `AssertConfigurationIsValid()`.


``` diff
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(/* ... */);
    }

-    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
+    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper autoMapper)
    {
+        autoMapper.Configuration.AssertConfigurationIsValid();
    }
}
```

You can also inject just the `IConfigurationProvider`, but beware that an interface with the same name exists in the Microsoft.Extensions.Configuration namespace so use a fully qualified name.

``` diff
-public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper autoMapper)
+public void Configure(IApplicationBuilder app, IHostingEnvironment env, AutoMapper.IConfigurationProvider autoMapper)
{
-    autoMapper.Configuration.AssertConfigurationIsValid();
+    autoMapper.AssertConfigurationIsValid();
}
```
