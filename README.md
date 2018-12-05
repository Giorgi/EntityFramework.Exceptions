![EntityFramework.Exceptions](Icon.png "EntityFramework.Exceptions")

# EntityFramework.Exceptions
Handle database errors easily when working with Entity Framework Core. Supports SQLServer, PostgreSQL and MySql

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](License.md)
[![AppVeyor](https://img.shields.io/appveyor/ci/Giorgi/EntityFramework-Exceptions.svg)](https://ci.appveyor.com/project/Giorgi/entityframework-exceptions)

### What does EntityFramework.Exceptions do?

When using Entity Framework Core for data access all database exceptions are wrapped in `DbUpdateException`. If you need to find 
whether the exception was caused by a unique constraint, value being too long or value missing for a required column you need to dig into 
the concrete `DbException` subclass instance and check the error code to determine the exact cause.

EntityFramework.Exceptions simplifies this by handling all the database specific details and throwing different exceptions. All you have
to do is inherit your `DbContext` from `ExceptionProcessorContext` and handle the exception(s) such as `UniqueConstraintException`,
`CannotInsertNullException`, `MaxLengthExceededException`, `NumericOverflowException` you need.

### How do I get started?
First, install the package corresponding to your database:

```
PM> Install-Package EntityFramework.Exceptions.SqlServer
```

```
PM> Install-Package EntityFramework.Exceptions.MySql
```

```
PM> Install-Package EntityFramework.Exceptions.PostgreSQL
```

Then inherit your DbContext from ExceptionProcessorContext

```
class DemoContext : ExceptionProcessorContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSale> ProductSale { get; set; }

    //More code
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
