using EntityFramework.Exceptions.Common;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class SqlServerTests : IClassFixture<DemoContextFixture>, IDisposable
    {
        private readonly DemoContextFixture fixture;

        public SqlServerTests(DemoContextFixture fixture)
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
        public void MaxLengthViolationThrowsMaxLengthExceededException()
        {
            fixture.Context.Products.Add(new Product { Name = new string('G', 20) });

            Assert.Throws<MaxLengthExceededException>(() => fixture.Context.SaveChanges());
        }

        [Fact]
        public void NumericOverflowViolationThrowsNumericOverflowException()
        {
            var product = new Product { Name = "GD" };
            fixture.Context.Products.Add(product);
            fixture.Context.ProductSales.Add(new ProductSale { Price = 3141.59265m, Product = product});

            Assert.Throws<NumericOverflowException>(() => fixture.Context.SaveChanges());
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

    public class DemoContextFixture : IDisposable
    {
        internal DemoContext Context { get; }

        public DemoContextFixture()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.{environment}.json", optional: true).Build();
            var connectionString = configuration.GetConnectionString("SqlServer");
            
            var builder = new DbContextOptionsBuilder<DemoContext>().UseSqlServer(connectionString).UseExceptionProcessor();

            Context = new DemoContext(builder.Options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
