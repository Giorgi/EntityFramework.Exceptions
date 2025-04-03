![EntityFramework.Exceptions](https://raw.githubusercontent.com/Giorgi/EntityFramework.Exceptions/main/Icon.png "EntityFramework.Exceptions")

# EntityFramework.Exceptions
Handle database errors easily when working with Entity Framework Core. Supports SQLServer, PostgreSQL, SQLite, Oracle and MySql

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square&logo=Apache)](License.md)
[![Target](https://img.shields.io/static/v1?label=target&message=net8.0&color=512bd4&logo=.net&style=flat-square)](https://dotnet.microsoft.com/en-us/)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/Giorgi/EntityFramework.Exceptions/dotnet.yml)
[![Coverage Status](https://img.shields.io/coveralls/github/Giorgi/EntityFramework.Exceptions?logo=coveralls&style=flat-square)](https://coveralls.io/github/Giorgi/EntityFramework.Exceptions)
[![Ko-Fi](https://img.shields.io/static/v1?style=flat-square&message=Support%20the%20Project&color=success&style=plastic&logo=ko-fi&label=$$)](https://ko-fi.com/U6U81LHU8)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=EntityFramework.Exceptions&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=EntityFramework.Exceptions)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=EntityFramework.Exceptions&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=EntityFramework.Exceptions)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=EntityFramework.Exceptions&metric=bugs)](https://sonarcloud.io/dashboard?id=EntityFramework.Exceptions)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=EntityFramework.Exceptions&metric=code_smells)](https://sonarcloud.io/dashboard?id=EntityFramework.Exceptions)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=EntityFramework.Exceptions&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=EntityFramework.Exceptions)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=EntityFramework.Exceptions&metric=coverage)](https://sonarcloud.io/dashboard?id=EntityFramework.Exceptions)

[![](https://img.shields.io/nuget/dt/EntityFrameworkCore.Exceptions.SqlServer.svg?label=EntityFrameworkCore.Exceptions.SqlServer&style=flat-square&logo=Microsoft-Sql-Server)](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.SqlServer/)
[![](https://img.shields.io/nuget/dt/EntityFrameworkCore.Exceptions.PostgreSQL.svg?label=EntityFrameworkCore.Exceptions.PostgreSQL&style=flat-square&logo=PostgreSQL)](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.PostgreSQL/)
[![](https://img.shields.io/nuget/dt/EntityFrameworkCore.Exceptions.MySQL.svg?label=EntityFrameworkCore.Exceptions.MySQL&style=flat-square&logo=MySQL&logoColor=white)](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.MySQL/)
[![](https://img.shields.io/nuget/dt/EntityFrameworkCore.Exceptions.MySQL.Pomelo.svg?label=EntityFrameworkCore.Exceptions.MySQL.Pomelo&style=flat-square&logo=MySQL&logoColor=white)](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.MySQL.Pomelo/)
[![](https://img.shields.io/nuget/dt/EntityFrameworkCore.Exceptions.Sqlite.svg?label=EntityFrameworkCore.Exceptions.Sqlite&style=flat-square&logo=Sqlite)](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.Sqlite/)
[![](https://img.shields.io/nuget/dt/EntityFrameworkCore.Exceptions.Oracle.svg?label=EntityFrameworkCore.Exceptions.Oracle&style=flat-square&logo=Oracle)](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.Oracle/)

### Entity Framework Community Standup Live Show

[![Entity Framework Community Standup - Typed Exceptions for Entity Framework Core](https://img.youtube.com/vi/aUl5QfswNU4/0.jpg)](https://www.youtube.com/watch?v=aUl5QfswNU4)

### OSS Power-Ups: EntityFramework.Exceptions

[![OSS Power-Ups: EntityFramework.Exceptions](https://img.youtube.com/vi/IwxqFd80Si8/0.jpg)](https://www.youtube.com/watch?v=IwxqFd80Si8)

### Nick Chapsas - The Smart Way to Handle EF Core Exceptions

[![Nick Chapsas - The Smart Way to Handle EF Core Exceptions](https://img.youtube.com/vi/QKwZlWvfh-o/0.jpg)](https://www.youtube.com/watch?v=QKwZlWvfh-o)

### What does EntityFramework.Exceptions do?

When using Entity Framework Core for data access all database exceptions are wrapped in `DbUpdateException`. If you need to find 
whether the exception was caused by a unique constraint, value being too long or value missing for a required column you need to dig into 
the concrete `DbException` subclass instance and check the error code to determine the exact cause.

EntityFramework.Exceptions simplifies this by handling all the database specific details and throwing different exceptions. All you have
to do is to configure `DbContext` by calling `UseExceptionProcessor` and handle the exception(s)  you need, such as `UniqueConstraintException`,
`CannotInsertNullException`, `MaxLengthExceededException`, `NumericOverflowException`, or `ReferenceConstraintException`.

In case of `UniqueConstraintException` and `ReferenceConstraintException` you can get the name of the associated constraint with **`ConstraintName`** property. The **`ConstraintProperties`** will contain the properties that are part of the constraint. 

> [!WARNING]
> `ConstraintName` and `ConstraintProperties` will be populated only if the index is defined in the Entity Framework Model. These properties will not be populated if the index exists in the database but isn't part of the model or if the index is added with [MigrationBuilder.Sql](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.migrations.migrationbuilder.sql?view=efcore-8.0) method.

> [!WARNING]
> `ConstraintName` and `ConstraintProperties` will not be populated when using SQLite. 

All these exceptions inherit from `DbUpdateException` for backwards compatibility.

### How do I get started?
First, install the package corresponding to your database:

```
dotnet add package EntityFrameworkCore.Exceptions.SqlServer
```

```
dotnet add package EntityFrameworkCore.Exceptions.MySql
```

```
dotnet add package EntityFrameworkCore.Exceptions.MySql.Pomelo
```

```
dotnet add package EntityFrameworkCore.Exceptions.PostgreSQL
```

```
dotnet add package EntityFrameworkCore.Exceptions.Sqlite
```

```
dotnet add package EntityFrameworkCore.Exceptions.Oracle
```

Next, in your DbContext `OnConfiguring` method call `UseExceptionProcessor` extension method:

```csharp
class DemoContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSale> ProductSale { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>().HasIndex(u => u.Name).IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}
```    

You will now start getting different exception for different database error. For example, when a unique constraints fails you will get `UniqueConstraintException` exception:

```csharp
using (var demoContext = new DemoContext())
{
    demoContext.Products.Add(new Product
    {
        Name = "demo",
        Price = 10
    });

    demoContext.Products.Add(new Product
    {
        Name = "demo",
        Price = 100
    });

    try
    {
        demoContext.SaveChanges();
    }
    catch (UniqueConstraintException e)
    {
        //Handle exception here
        Console.WriteLine($"Unique constraint {e.ConstraintName} violated. Duplicate value for {e.ConstraintProperties[0]}");
    }
}
```
> [!TIP]
> If you want to use another native SQLite binary instead of `e_sqlite3.dll` use the [EntityFrameworkCore.Exceptions.Sqlite.Core](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.Sqlite.Core) package. This package depends on Microsoft.Data.Sqlite.Core package, which doesn't include SQLite native binary so you can use any native binary you want.

### Usage with DbContext pooling

Instead of calling `UseExceptionProcessor` in the `OnConfiguring` method, add it where you add your `DbContextPool`:

```csharp
// Replace UseNpgsql with the sql flavor you're using
builder.Services.AddDbContextPool<DemoContext>(options => options
    .UseNpgsql(config.GetConnectionString("DemoConnection"))
    .UseExceptionProcessor());
```
