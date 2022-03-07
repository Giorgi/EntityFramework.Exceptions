using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public abstract class DatabaseTests : IDisposable
    {
        internal DemoContext Context { get; }

        protected DatabaseTests(DemoContext context)
        {
            Context = context;
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
            Context.Products.Add(new Product { Name = new string('G', 20) });

            Assert.Throws<MaxLengthExceededException>(() => Context.SaveChanges());
            await Assert.ThrowsAsync<MaxLengthExceededException>(() => Context.SaveChangesAsync());
        }

        [Fact]
        public virtual async Task NumericOverflowViolationThrowsNumericOverflowException()
        {
            var product = new Product { Name = "GD" };
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
        public virtual async Task NotHandledViolationReThrowsOriginalException()
        {
            var product = new Product { Name = "GD" };
            Context.Products.Add(product);

            Context.SaveChanges();
            Context.Database.ExecuteSqlInterpolated(MySqlDatabaseFacadeExtensions.IsMySql(Context.Database) || MySQLDatabaseFacadeExtensions.IsMySql(Context.Database)
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
            var productPriceHistory = new ProductPriceHistory { Product = product, Price = 15.27m, EffectiveDate = DateTime.Now.Date.AddDays(-10) };
            Context.ProductPriceHistories.Add(productPriceHistory);
            await Context.SaveChangesAsync();

            CleanupContext();

            product = Context.Products.Find(product.Id);
            Context.Products.Remove(product);

            Assert.Throws<ReferenceConstraintException>(() => Context.SaveChanges());
            await Assert.ThrowsAsync<ReferenceConstraintException>(() => Context.SaveChangesAsync());
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
}