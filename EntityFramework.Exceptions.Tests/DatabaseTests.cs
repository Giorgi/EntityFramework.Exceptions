using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using MySql.EntityFrameworkCore.Extensions;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public abstract class DatabaseTests : IDisposable
{
    private readonly bool isMySql;
    internal DemoContext Context { get; }

    protected DatabaseTests(DemoContext context)
    {
        Context = context;
        isMySql = MySqlDatabaseFacadeExtensions.IsMySql(Context.Database) || MySQLDatabaseFacadeExtensions.IsMySql(Context.Database);
    }

    [Fact]
    public virtual async Task UniqueColumnViolationThrowsUniqueConstraintException()
    {
        Context.Products.Add(new Product { Name = "GD" });
        Context.Products.Add(new Product { Name = "GD" });

        Assert.Throws<UniqueConstraintException>(() => Context.SaveChanges());
        await Assert.ThrowsAsync<UniqueConstraintException>(() => Context.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task PrimaryKeyViolationThrowsUniqueConstraintException()
    {
        var product1 = new Product { Name = "GD", Id = 42 };
        var product2 = new Product { Name = "GD", Id = 42 };

        Context.Products.Add(product1);
        Context.SaveChanges();

        CleanupContext();

        Context.Products.Add(product2);
        Assert.Throws<UniqueConstraintException>(() => Context.SaveChanges());
        await Assert.ThrowsAsync<UniqueConstraintException>(() => Context.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task RequiredColumnViolationThrowsCannotInsertNullException()
    {
        Context.Products.Add(new Product());

        Assert.Throws<CannotInsertNullException>(() => Context.SaveChanges());
        await Assert.ThrowsAsync<CannotInsertNullException>(() => Context.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task MaxLengthViolationThrowsMaxLengthExceededException()
    {
        Context.Products.Add(new Product { Name = new string('G', DemoContext.ProductNameMaxLength + 5) });

        Assert.Throws<MaxLengthExceededException>(() => Context.SaveChanges());
        await Assert.ThrowsAsync<MaxLengthExceededException>(() => Context.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task NumericOverflowViolationThrowsNumericOverflowException()
    {
        var product = new Product { Name = "Numeric Overflow Test" };
        Context.Products.Add(product);
        Context.ProductSales.Add(new ProductSale { Price = 3141.59265m, Product = product });

        Assert.Throws<NumericOverflowException>(() => Context.SaveChanges());
        await Assert.ThrowsAsync<NumericOverflowException>(() => Context.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task ReferenceViolationThrowsReferenceConstraintException()
    {
        Context.ProductSales.Add(new ProductSale { Price = 3.14m });

        Assert.Throws<ReferenceConstraintException>(() => Context.SaveChanges());
        await Assert.ThrowsAsync<ReferenceConstraintException>(() => Context.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task DatabaseUnrelatedExceptionThrowsOriginalException()
    {
        var product = new Product { Name = "Unhandled Violation Test" };
        Context.Products.Add(product);

        Context.SaveChanges();
        Context.Database.ExecuteSqlInterpolated(isMySql
            ? $"Delete from products where id={product.Id}"
            : (FormattableString)$"Delete from \"Products\" where \"Id\"={product.Id}");
        product.Name = "G";

        Assert.ThrowsAny<DbUpdateException>(() => Context.SaveChanges());
        await Assert.ThrowsAnyAsync<DbUpdateException>(() => Context.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task DeleteParentItemThrowsReferenceConstraintException()
    {
        var product = new Product { Name = "AN" };
        var productPriceHistory = new ProductPriceHistory { Product = product, Price = 15.27m, EffectiveDate = DateTimeOffset.UtcNow.Date.AddDays(-10) };
        Context.ProductPriceHistories.Add(productPriceHistory);
        await Context.SaveChangesAsync();

        CleanupContext();

        product = Context.Products.Find(product.Id);
        Context.Products.Remove(product);

        Assert.Throws<ReferenceConstraintException>(() => Context.SaveChanges());
        await Assert.ThrowsAsync<ReferenceConstraintException>(() => Context.SaveChangesAsync());
    }

    [Fact]
    public async Task NotHandledViolationReThrowsOriginalException()
    {
        Context.Customers.Add(new Customer { Fullname = "Test" });

        await Context.Database.ExecuteSqlRawAsync(isMySql ? "Drop table customers" : "Drop table \"Customers\"");

        Assert.Throws<DbUpdateException>(() => Context.SaveChanges());
        await Assert.ThrowsAsync<DbUpdateException>(() => Context.SaveChangesAsync());
    }

    public virtual void Dispose()
    {
        CleanupContext();
    }

    protected void CleanupContext()
    {
        foreach (var entityEntry in Context.ChangeTracker.Entries())
        {
            entityEntry.State = EntityState.Detached;
        }
    }
}