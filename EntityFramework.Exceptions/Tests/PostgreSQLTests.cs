using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class PostgreSQLTests : DatabaseTests, IClassFixture<PostgreSQLDemoContextFixture>
    {
        public PostgreSQLTests(PostgreSQLDemoContextFixture fixture) : base(fixture.DemoContext, fixture.SameNameIndexesContext)
        {
        }

        [Fact]
        public async Task SaveChangesDeadlockThrowsDeadlockException()
        {
            var seed1 = DemoContext.Products.Add(new() { Name = "DLPgSave1" });
            var seed2 = DemoContext.Products.Add(new() { Name = "DLPgSave2" });
            await DemoContext.SaveChangesAsync();

            var id1 = seed1.Entity.Id;
            var id2 = seed2.Entity.Id;

            DemoContext.ChangeTracker.Clear();

            await using var controlContext = new DemoContext(DemoContext.Options);
            await using var tx1 = await DemoContext.Database.BeginTransactionAsync();
            await using var tx2 = await controlContext.Database.BeginTransactionAsync();

            var p1Lock = new Product { Id = id1, Name = "DLPgSave1a" };
            DemoContext.Attach(p1Lock).Property(p => p.Name).IsModified = true;
            await DemoContext.SaveChangesAsync();

            var p2Lock = new Product { Id = id2, Name = "DLPgSave2a" };
            controlContext.Attach(p2Lock).Property(p => p.Name).IsModified = true;
            await controlContext.SaveChangesAsync();

            var p2Cross = new Product { Id = id2, Name = "DLPgSave2b" };
            DemoContext.Attach(p2Cross).Property(p => p.Name).IsModified = true;

            var p1Cross = new Product { Id = id1, Name = "DLPgSave1b" };
            controlContext.Attach(p1Cross).Property(p => p.Name).IsModified = true;

            await Assert.ThrowsAsync<DeadlockException>(() => Task.WhenAll(
                Task.Run(() => DemoContext.SaveChangesAsync()),
                Task.Run(() => controlContext.SaveChangesAsync())));
        }
    }

    public class PostgreSQLDemoContextFixture : DemoContextFixture<PostgreSqlContainer>
    {
        private static readonly ExceptionProcessorOptions Options = new() { HandleDeadlock = true };

        static PostgreSQLDemoContextFixture()
        {
            Container = new PostgreSqlBuilder().Build();
        }

        protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString)
            => builder.UseNpgsql(connectionString).UseExceptionProcessor(Options);

        protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, string connectionString)
            => builder.UseNpgsql(connectionString).UseExceptionProcessor(Options);
    }
}
