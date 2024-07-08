using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EFExceptionSchema.Entities.Incidents;
using EntityFramework.Exceptions.Tests.ConstraintTests;
using MySql.EntityFrameworkCore.Extensions;
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
        Assert.Throws<UniqueConstraintException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<UniqueConstraintException>(() => DemoContext.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task RequiredColumnViolationThrowsCannotInsertNullException()
    {
        DemoContext.Products.Add(new Product());

        Assert.Throws<CannotInsertNullException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<CannotInsertNullException>(() => DemoContext.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task MaxLengthViolationThrowsMaxLengthExceededException()
    {
        DemoContext.Products.Add(new Product { Name = new string('G', DemoContext.ProductNameMaxLength + 5) });

        Assert.Throws<MaxLengthExceededException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<MaxLengthExceededException>(() => DemoContext.SaveChangesAsync());
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
    public virtual async Task DatabaseUnrelatedExceptionThrowsOriginalException()
    {
        var product = new Product { Name = "Unhandled Violation Test" };
        DemoContext.Products.Add(product);

        DemoContext.SaveChanges();
        DemoContext.Database.ExecuteSqlInterpolated(isMySql
            ? $"Delete from products where id={product.Id}"
            : (FormattableString)$"Delete from \"Products\" where \"Id\"={product.Id}");
        product.Name = "G";

        Assert.ThrowsAny<DbUpdateException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAnyAsync<DbUpdateException>(() => DemoContext.SaveChangesAsync());
    }

    [Fact]
    public virtual async Task DeleteParentItemThrowsReferenceConstraintException()
    {
        var product = new Product { Name = "AN" };
        var productPriceHistory = new ProductPriceHistory { Product = product, Price = 15.27m, EffectiveDate = DateTimeOffset.UtcNow.Date.AddDays(-10) };
        DemoContext.ProductPriceHistories.Add(productPriceHistory);
        await DemoContext.SaveChangesAsync();

        CleanupContext();

        product = DemoContext.Products.Find(product.Id);
        DemoContext.Products.Remove(product);

        Assert.Throws<ReferenceConstraintException>(() => DemoContext.SaveChanges());
        await Assert.ThrowsAsync<ReferenceConstraintException>(() => DemoContext.SaveChangesAsync());
    }

    [Fact]
    public async Task NotHandledViolationReThrowsOriginalException()
    {
        DemoContext.Customers.Add(new Customer { Fullname = "Test" });

        await DemoContext.Database.ExecuteSqlRawAsync(isMySql ? "Drop table customers" : "Drop table \"Customers\"");

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