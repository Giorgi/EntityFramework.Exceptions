using EFExceptionSchema.Entities.Incidents;
using EntityFramework.Exceptions.Common;
using EntityFramework.Exceptions.Tests.ConstraintTests;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public abstract class DatabaseTests : IDisposable
{
    private readonly bool isMySql;
    private readonly bool isSqlite;
    internal DemoContext DemoContext { get; }
    internal SameNameIndexesContext SameNameIndexesContext { get; }

    protected DatabaseTests(DemoContext demoContext, SameNameIndexesContext sameNameIndexesContext = null)
    {
        DemoContext = demoContext;
        SameNameIndexesContext = sameNameIndexesContext;

        isMySql = MySqlDatabaseFacadeExtensions.IsMySql(DemoContext.Database) || MySQLDatabaseFacadeExtensions.IsMySql(DemoContext.Database);
        isSqlite = demoContext.Database.IsSqlite();
    }

    [Fact]
    public virtual async Task UniqueColumnViolationThrowsUniqueConstraintException()
    {
        DemoContext.Products.Add(new Product { Name = "GD" });
        DemoContext.Products.Add(new Product { Name = "GD" });

        var uniqueConstraintException = Assert.Throws<UniqueConstraintException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<UniqueConstraintException>(() => DemoContext.SaveChangesAsync());

        if (!isSqlite)
        {
            Assert.False(string.IsNullOrEmpty(uniqueConstraintException.ConstraintName));
            Assert.NotEmpty(uniqueConstraintException.ConstraintProperties);
            Assert.Contains<string>(nameof(Product.Name), uniqueConstraintException.ConstraintProperties);
            Assert.Equal(nameof(DemoContext.Products), uniqueConstraintException.SchemaQualifiedTableName);
        }
    }

    [Fact]
    public virtual async Task UniqueColumnViolationThrowsUniqueConstraintExceptionThroughExecuteUpdate()
    {
        DemoContext.Products.Add(new Product { Name = "Bulk Update 1" });
        DemoContext.Products.Add(new Product { Name = "Bulk Update 2" });

        await DemoContext.SaveChangesAsync();
        Assert.Throws<UniqueConstraintException>(() => DemoContext.Products.ExecuteUpdate(p => p.SetProperty(pp => pp.Name, "Bulk Update 1")));
        await Assert.ThrowsAsync<UniqueConstraintException>(async () => await DemoContext.Products.ExecuteUpdateAsync(p => p.SetProperty(pp => pp.Name, "Bulk Update 1")));
        await DemoContext.Products
            .Where(p => p.Name == "Bulk Update 1" || p.Name == "Bulk Update 2")
            .ExecuteDeleteAsync();
    }

    [Fact]
    public virtual async Task UniqueColumnViolationSameNamesIndexesInDifferentSchemasSetsCorrectTableName()
    {
        if (SameNameIndexesContext == null)
        {
            Assert.True(SameNameIndexesContext == null);
            return;
        }

        SameNameIndexesContext.IncidentCategories.Add(new EFExceptionSchema.Entities.Incidents.Category
        {
            Name = "Rope Access"
        });
        
        SameNameIndexesContext.IncidentCategories.Add(new EFExceptionSchema.Entities.Incidents.Category
        {
            Name = "Rope Access"
        });

        var uniqueConstraintException = Assert.Throws<UniqueConstraintException>(() => SameNameIndexesContext.SaveChanges());
        await Assert.ThrowsAsync<UniqueConstraintException>(() => SameNameIndexesContext.SaveChangesAsync());

        if (!isSqlite)
        {
            Assert.False(string.IsNullOrEmpty(uniqueConstraintException.ConstraintName));
            Assert.NotEmpty(uniqueConstraintException.ConstraintProperties);
            Assert.Contains<string>(nameof(Category.Name), uniqueConstraintException.ConstraintProperties);
            Assert.Equal("Incidents.Category", uniqueConstraintException.SchemaQualifiedTableName);
        }
    }

    [Fact]
    public virtual async Task PrimaryKeyViolationThrowsUniqueConstraintException()
    {
        var product1 = new Product { Name = "GD", Id = 42 };
        var product2 = new Product { Name = "GD", Id = 42 };

        DemoContext.Products.Add(product1);
        DemoContext.SaveChanges();

        CleanupContext();

        DemoContext.Products.Add(product2);
        var uniqueConstraintException = Assert.Throws<UniqueConstraintException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<UniqueConstraintException>(() => DemoContext.SaveChangesAsync());

        if (!isSqlite && !isMySql)
        {
            Assert.False(string.IsNullOrEmpty(uniqueConstraintException.ConstraintName));
            Assert.False(string.IsNullOrEmpty(uniqueConstraintException.SchemaQualifiedTableName));
            Assert.NotEmpty(uniqueConstraintException.ConstraintProperties);
            Assert.Contains<string>(nameof(Product.Id), uniqueConstraintException.ConstraintProperties); 
        }
    }

    [Fact]
    public virtual async Task RequiredColumnViolationThrowsCannotInsertNullException()
    {
        DemoContext.Products.Add(new Product());

        Assert.Throws<CannotInsertNullException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<CannotInsertNullException>(() => DemoContext.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task RequiredColumnViolationThrowsCannotInsertNullExceptionThroughExecuteUpdate()
    {
        DemoContext.Products.Add(new Product { Name = "Bulk Update 1" });
        await DemoContext.SaveChangesAsync();

        Assert.Throws<CannotInsertNullException>(() => DemoContext.Products.ExecuteUpdate(p => p.SetProperty(pp => pp.Name, (string)null)));
        await Assert.ThrowsAsync<CannotInsertNullException>(async () => await DemoContext.Products.ExecuteUpdateAsync(p => p.SetProperty(pp => pp.Name, (string)null)));
        await DemoContext.Products.Where(p => p.Name == "Bulk Update 1").ExecuteDeleteAsync();
    }

    [Fact]
    public virtual async Task MaxLengthViolationThrowsMaxLengthExceededException()
    {
        DemoContext.Products.Add(new Product { Name = new string('G', DemoContext.ProductNameMaxLength + 5) });

        Assert.Throws<MaxLengthExceededException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<MaxLengthExceededException>(() => DemoContext.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task MaxLengthViolationThrowsMaxLengthExceededExceptionThroughExecuteUpdate()
    {
        DemoContext.Products.Add(new Product { Name = "Bulk Update 1" });
        await DemoContext.SaveChangesAsync();

        CleanupContext();

        Assert.Throws<MaxLengthExceededException>(Query);
        await Assert.ThrowsAsync<MaxLengthExceededException>(QueryAsync);

        await DemoContext.Products
            .Where(p => p.Name == "Bulk Update 1")
            .ExecuteDeleteAsync();

        return;

        void Query()
        {
            DemoContext.Products
                .Where(p => p.Name == "Bulk Update 1")
                .ExecuteUpdate(p => p.SetProperty(pp => pp.Name, new string('G', DemoContext.ProductNameMaxLength + 5)));
        }

        async Task QueryAsync()
        {
            await DemoContext.Products
                .Where(p => p.Name == "Bulk Update 1")
                .ExecuteUpdateAsync(p => p.SetProperty(pp => pp.Name, new string('G', DemoContext.ProductNameMaxLength + 5)));
        }
    }

    [Fact]
    public virtual async Task NumericOverflowViolationThrowsNumericOverflowException()
    {
        var product = new Product { Name = "Numeric Overflow Test" };
        DemoContext.Products.Add(product);
        DemoContext.ProductSales.Add(new ProductSale { Price = 3141.59265m, Product = product });

        Assert.Throws<NumericOverflowException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<NumericOverflowException>(() => DemoContext.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task NumericOverflowViolationThrowsNumericOverflowExceptionThroughExecuteUpdate()
    {
        var product = new Product { Name = "Numeric Overflow Test 2" };
        DemoContext.Products.Add(product);
        var sale = new ProductSale { Price = 1m, Product = product };
        DemoContext.ProductSales.Add(sale);
        await DemoContext.SaveChangesAsync();

        Assert.Throws<NumericOverflowException>(Query);
        await Assert.ThrowsAsync<NumericOverflowException>(QueryAsync);

        DemoContext.Remove(sale);
        DemoContext.Remove(product);
        await DemoContext.SaveChangesAsync();

        return;

        void Query()
        {
            DemoContext.ProductSales
                .Where(s => s.Id == sale.Id)
                .ExecuteUpdate(s => s.SetProperty(ss => ss.Price, 3141.59265m));
        }

        async Task QueryAsync()
        {
            await DemoContext.ProductSales
                .Where(s => s.Id == sale.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(ss => ss.Price, 3141.59265m));
        }
    }

    [Fact]
    public virtual async Task ReferenceViolationThrowsReferenceConstraintException()
    {
        DemoContext.ProductSales.Add(new ProductSale { Price = 3.14m });

        var referenceConstraintException = Assert.Throws<ReferenceConstraintException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<ReferenceConstraintException>(() => DemoContext.SaveChangesAsync());

        if (!isSqlite)
        {
            Assert.False(string.IsNullOrEmpty(referenceConstraintException.ConstraintName));
            Assert.NotEmpty(referenceConstraintException.ConstraintProperties);
            Assert.Contains<string>(nameof(ProductSale.ProductId), referenceConstraintException.ConstraintProperties);
        }
    }

    [Fact]
    public virtual async Task ReferenceViolationThrowsReferenceConstraintExceptionThroughExecuteUpdate()
    {
        var product = new Product { Name = "RefConstraint Violation 1" };
        DemoContext.Products.Add(product);
        var sale = new ProductSale { Price = 1m, Product = product };
        DemoContext.ProductSales.Add(sale);
        await DemoContext.SaveChangesAsync();

        var exception = Assert.Throws<ReferenceConstraintException>(Query);
        var asyncException = await Assert.ThrowsAsync<ReferenceConstraintException>(QueryAsync);

        if (!isSqlite)
        {
            Assert.False(string.IsNullOrEmpty(exception.ConstraintName));
            Assert.NotEmpty(exception.ConstraintProperties);
            Assert.Contains<string>(nameof(ProductSale.ProductId), exception.ConstraintProperties);

            Assert.False(string.IsNullOrEmpty(asyncException.ConstraintName));
            Assert.NotEmpty(asyncException.ConstraintProperties);
            Assert.Contains<string>(nameof(ProductSale.ProductId), asyncException.ConstraintProperties);
        }

        return;

        void Query()
        {
            DemoContext.ProductSales
                .Where(s => s.Id == sale.Id)
                .ExecuteUpdate(s => s.SetProperty(ss => ss.ProductId, 0));
        }

        async Task QueryAsync()
        {
            await DemoContext.ProductSales
                .Where(s => s.Id == sale.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(ss => ss.ProductId, 0));
        }
    }

    [Fact]
    public virtual async Task DatabaseUnrelatedExceptionThrowsOriginalException()
    {
        var product = new Product { Name = "Unhandled Violation Test" };
        DemoContext.Products.Add(product);

        DemoContext.SaveChanges();
        DemoContext.Database.ExecuteSqlInterpolated(isMySql
            ? $"Delete from Products where id={product.Id}"
            : (FormattableString)$"Delete from \"Products\" where \"Id\"={product.Id}");
        product.Name = "G";

        Assert.ThrowsAny<DbUpdateException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAnyAsync<DbUpdateException>(() => DemoContext.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task DeleteParentItemThrowsReferenceConstraintException()
    {
        var product = new Product { Name = "AN" };
        var productPriceHistory = new ProductPriceHistory { Product = product, Price = 15.27m, EffectiveDate = DateTimeOffset.UtcNow };
        DemoContext.ProductPriceHistories.Add(productPriceHistory);
        await DemoContext.SaveChangesAsync();

        CleanupContext();

        product = DemoContext.Products.Find(product.Id);
        DemoContext.Products.Remove(product);

        Assert.Throws<ReferenceConstraintException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<ReferenceConstraintException>(() => DemoContext.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task DeleteParentItemThrowsReferenceConstraintExceptionThroughExecuteDelete()
    {
        var product = new Product { Name = "AN2" };
        var productPriceHistory = new ProductPriceHistory { Product = product, Price = 15.27m, EffectiveDate = DateTimeOffset.UtcNow };
        DemoContext.ProductPriceHistories.Add(productPriceHistory);
        await DemoContext.SaveChangesAsync();

        CleanupContext();

        Assert.Throws<ReferenceConstraintException>(Query);
        await Assert.ThrowsAsync<ReferenceConstraintException>(QueryAsync);

        return;

        void Query()
        {
            DemoContext.Products
                .Where(p => p.Name == "AN2")
                .ExecuteDelete();
        }

        async Task QueryAsync()
        {
            await DemoContext.Products
                .Where(p => p.Name == "AN2")
                .ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async Task NotHandledViolationReThrowsOriginalException()
    {
        DemoContext.Customers.Add(new Customer { Fullname = "Test" });

        await DemoContext.Database.ExecuteSqlRawAsync(isMySql ? "Drop table Customers" : "Drop table \"Customers\"");

        Assert.Throws<DbUpdateException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<DbUpdateException>(() => DemoContext.SaveChangesAsync());
    }

    public virtual void Dispose()
    {
        CleanupContext();
    }

    protected void CleanupContext()
    {
        foreach (var entityEntry in DemoContext.ChangeTracker.Entries())
        {
            entityEntry.State = EntityState.Detached;
        }
    }
}