# AutoMapper extensions for Microsoft.Extensions.DependencyInjection

[![CI](https://github.com/automapper/automapper.extensions.microsoft.dependencyinjection/workflows/CI/badge.svg)](https://github.com/automapper/automapper.extensions.microsoft.dependencyinjection/workflows/CI)
[![NuGet](http://img.shields.io/nuget/v/automapper.extensions.microsoft.dependencyinjection.svg?label=NuGet)](https://www.nuget.org/packages/automapper.extensions.microsoft.dependencyinjection/)
[![MyGet (dev)](https://img.shields.io/myget/automapperdev/vpre/automapper.extensions.microsoft.dependencyinjection.svg?label=MyGet)](https://myget.org/feed/automapperdev/package/nuget/AutoMapper.Extensions.Microsoft.DependencyInjection)

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
- As a transient instance for `IMapper`
- `ITypeConverter` instances as transient
- `IValueConverter` instances as transient
- `IValueResolver` instances as transient
- `IMemberValueResolver` instances as transient
- `IMappingAction` instances as transient

Mapping configuration is static as it is the root object that can create an `IMapper`.

Mapper instances are registered as transient. You can configure this with the `serviceLifetime` parameter. Be careful changing this, as `Mapper` takes a dependency on a factory method to instantiate the other extensions.

### Mapper.Map usage

To map at runtime, add a dependency on `IMapper`:

```c#
public class EmployeesController {
	private readonly IMapper _mapper;

	public EmployeesController(IMapper mapper)
		=> _mapper = mapper;

	// use _mapper.Map to map
}
```

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
                       .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
					   .ToListAsync();
```
