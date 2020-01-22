![EntityFramework.Exceptions](Icon.png "EntityFramework.Exceptions")

# EntityFramework.Exceptions
Handle database errors easily when working with Entity Framework Core. Supports SQLServer, PostgreSQL and MySql

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](License.md)
[![AppVeyor](https://img.shields.io/appveyor/ci/Giorgi/EntityFramework-Exceptions.svg)](https://ci.appveyor.com/project/Giorgi/entityframework-exceptions)

[![](https://img.shields.io/nuget/dt/EntityFrameworkCore.Exceptions.SqlServer.svg?label=EntityFrameworkCore.Exceptions.SqlServer)](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.SqlServer/)
[![](https://img.shields.io/nuget/dt/EntityFrameworkCore.Exceptions.PostgreSQL.svg?label=EntityFrameworkCore.Exceptions.PostgreSQL)](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.PostgreSQL/)
[![](https://img.shields.io/nuget/dt/EntityFrameworkCore.Exceptions.MySQL.svg?label=EntityFrameworkCore.Exceptions.MySQL)](https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.MySQL/)

### What does EntityFramework.Exceptions do?

When using Entity Framework Core for data access all database exceptions are wrapped in `DbUpdateException`. If you need to find 
whether the exception was caused by a unique constraint, value being too long or value missing for a required column you need to dig into 
the concrete `DbException` subclass instance and check the error code to determine the exact cause.

EntityFramework.Exceptions simplifies this by handling all the database specific details and throwing different exceptions. All you have
to do is to configure `DbContext` by calling `UseExceptionProcessor` and handle the exception(s) such as `UniqueConstraintException`,
`CannotInsertNullException`, `MaxLengthExceededException`, `NumericOverflowException`, `ReferenceConstraintException` you need.

### How do I get started?
First, install the package corresponding to your database:

```
PM> Install-Package EntityFrameworkCore.Exceptions.SqlServer
```

```
PM> Install-Package EntityFrameworkCore.Exceptions.MySql
```

```
PM> Install-Package EntityFrameworkCore.Exceptions.PostgreSQL
```

Then in your DbContext `OnConfiguring` method call `UseExceptionProcessor` extension method:

```
class DemoContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSale> ProductSale { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}
```    

You will now start getting different exception for different database errors. For example when a unique constraints fails you will get `UniqueConstraintException` exception:

```
using (var demoContext = new DemoContext())
{
    demoContext.Products.Add(new Product
    {
        Name = "a",
        Price = 1
    });

    demoContext.Products.Add(new Product
    {
        Name = "a",
        Price = 1
    });

    try
    {
        demoContext.SaveChanges();
    }
    catch (UniqueConstraintException e)
    {
        //Handle exception here
    }
}
```
