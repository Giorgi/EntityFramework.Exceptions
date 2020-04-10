using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public abstract class DatabaseTests
    {
        internal DemoContext Context { get; }
        internal DbContextOptions<DemoContext> ContextOptions { get; }

        protected DatabaseTests(DbContextOptions<DemoContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Context = GetNewContext();
        }

        private DemoContext GetNewContext()
        {
            return new DemoContext(ContextOptions);
        }

        [Fact]
        public virtual void UniqueColumnViolationThrowsUniqueConstraintException()
        {
            Context.Products.Add(new Product { Name = "GD" });
            Context.Products.Add(new Product { Name = "GD" });

            Assert.Throws<UniqueConstraintException>(() => Context.SaveChanges());
        }

        [Fact]
        public virtual void PrimaryKeyViolationThrowsUniqueConstraintException()
        {
            var product1 = new Product { Name = "PROD1", Id = 42 };
            var product2 = new Product { Name = "PROD2", Id = 42 };

            var context1 = GetNewContext();
            context1.Products.Add(product1);
            context1.SaveChanges();

            var context2 = GetNewContext();
            context2.Products.Add(product2);
            Assert.Throws<UniqueConstraintException>(() => context2.SaveChanges());
        }

        [Fact]
        public virtual void PrimaryCompositeKeyViolationThrowsUniqueConstraintException()
        {
            var product = new Product { Name = "GD" };
            Context.Products.Add(product);
            var productSale = new ProductSale { Price = 42, Product = product };
            Context.ProductSales.Add(productSale);

            var item1 = new CompositeKeyItem
                {ProductId = product.Id, ProductSaleId = productSale.Id};
            var item2 = new CompositeKeyItem
                {ProductId = product.Id, ProductSaleId = productSale.Id};

            var context1 = GetNewContext();
            context1.CompositeKeyItems.Add(item1);
            context1.SaveChanges();

            var context2 = GetNewContext();
            context2.CompositeKeyItems.Add(item2);
            Assert.Throws<UniqueConstraintException>(() => context2.SaveChanges());
        }

        [Fact]
        public virtual void RequiredColumnViolationThrowsCannotInsertNullException()
        {
            Context.Products.Add(new Product());

            Assert.Throws<CannotInsertNullException>(() => Context.SaveChanges());
        }

        [Fact]
        public virtual void MaxLengthViolationThrowsMaxLengthExceededException()
        {
            Context.Products.Add(new Product { Name = new string('G', 20) });

            Assert.Throws<MaxLengthExceededException>(() => Context.SaveChanges());
        }

        [Fact]
        public virtual void NumericOverflowViolationThrowsNumericOverflowException()
        {
            var product = new Product { Name = "GD" };
            Context.Products.Add(product);
            Context.ProductSales.Add(new ProductSale { Price = 3141.59265m, Product = product });

            Assert.Throws<NumericOverflowException>(() => Context.SaveChanges());
        }

        [Fact]
        public virtual void ReferenceViolationThrowsReferenceConstraintException()
        {
            Context.ProductSales.Add(new ProductSale { Price = 3.14m });

            Assert.Throws<ReferenceConstraintException>(() => Context.SaveChanges());
        }

        public virtual void Dispose()
        {
            foreach (var entityEntry in Context.ChangeTracker.Entries())
            {
                entityEntry.State = EntityState.Detached;
            }

            Context.Products.RemoveRange(Context.Products);
            Context.SaveChanges();
        }
    }
}