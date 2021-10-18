# C3D.Core.DataAccess.Extensions.EF6

[![Build Status](https://dev.azure.com/flexviews/C3D%20Core/_apis/build/status/C3D%20Core?branchName=main)](https://dev.azure.com/flexviews/C3D%20Core/_build/latest?definitionId=42&branchName=main)
[![NuGet package](https://img.shields.io/nuget/v/C3D.Core.DataAccess.Extensions.EF6.svg)](https://nuget.org/packages/C3D.Core.DataAccess.Extensions.EF6)
[![NuGet downloads](https://img.shields.io/nuget/dt/C3D.Core.DataAccess.Extensions.EF6.svg)](https://nuget.org/packages/C3D.Core.DataAccess.Extensions.EF6)

This project contains the code for some EF6 extensions which make it easier to scan assemblies for entity model types, and for `ComplexTypeConfiguration<>` and `EntityTypeConfiguration<>` types.

## Quick Summary

- Install the Nuget package or copy this code into your project.
- Ensure your DBContext class has `using System.Data.Entity.ModelConfiguration;`
- In your `OnModelCreating` method use the extension methods

```cs
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.Configurations.GetTypesFromAssemblyType<C3D.Core.Models.Audit.Conventions.AuditTypesDefaultsConvention>().Add();
}
```

In this example the generic type is a type from the assembly I want to import from.

There are quite a few overloads/variations and parameters. The code documents _some_ of them.

An example is
```cs
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.Configurations.AddFromAssembly(typeof(SomeRandomType).Assembly,"My.Random.NameSpace");
}
```
This will add all CTC and ETC types from the assembly containing `SomeRandomType` where the configuration type itself _or_ the entity model type it is configuring is in the namespace `My.Random.NameSpace`.

e.g. this will match both
```cs
namespace My.Random.NameSpace.ETC {
    public class MyTypeConfiguration : EntityTypeConfiguration<My.Model.NameSpace.MyType> {

    }
}
```
and
```cs
namespace My.Configuration.NameSpace {
    public class MyTypeConfiguration : EntityTypeConfiguration<My.Random.NameSpace.Models.MyType> {

    }
}
```

## Example

```cs
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    List<ITypeInfo> typeInfos = new();
    typeInfos.AddRange(modelBuilder.Configurations.GetTypesFromAssemblyType<C3D.Core.Models.Audit.Conventions.AuditTypesDefaultsConvention>());
    typeInfos.AddRange(modelBuilder.Configurations.GetTypesFromAssemblyType(this.GetType()));
    typeInfos.ForEach(typeInfo => Log.InfoFormat("Detected TC {0}", typeInfo));
    typeInfos.Add();
}
```

## TODO

Better documentation, code documentation and add tests