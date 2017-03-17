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
