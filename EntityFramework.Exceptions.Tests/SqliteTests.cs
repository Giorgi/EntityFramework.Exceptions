using EntityFramework.Exceptions.Common;
using EntityFramework.Exceptions.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class SqliteTests : IClassFixture<SqliteDemoContextFixture>, IDisposable
    {
        private readonly DemoContextFixture fixture;

        public SqliteTests(SqliteDemoContextFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void UniqueColumnViolationThrowsUniqueConstraintException()
        {
            fixture.Context.Products.Add(new Product { Name = "GD" });
            fixture.Context.Products.Add(new Product { Name = "GD" });

            Assert.Throws<UniqueConstraintException>(() => fixture.Context.SaveChanges());
        }

        [Fact]
        public void RequiredColumnViolationThrowsCannotInsertNullException()
        {
            fixture.Context.Products.Add(new Product());

            Assert.Throws<CannotInsertNullException>(() => fixture.Context.SaveChanges());
        }

        [Fact]
        public void ReferenceViolationThrowsReferenceConstraintException()
        {
            fixture.Context.ProductSales.Add(new ProductSale { Price = 3.14m});

            Assert.Throws<ReferenceConstraintException>(() => fixture.Context.SaveChanges());
        }

        public void Dispose()
        {
            foreach (var entityEntry in fixture.Context.ChangeTracker.Entries())
            {
                entityEntry.State = EntityState.Detached;
            }
        }
    }

    public class SqliteDemoContextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            return builder.UseSqlite(configuration.GetConnectionString("Sqlite")).UseExceptionProcessor();
        }
    }
}
