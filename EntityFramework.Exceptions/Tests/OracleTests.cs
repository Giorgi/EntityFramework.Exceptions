using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using EntityFramework.Exceptions.Oracle;
using Microsoft.EntityFrameworkCore;
using Testcontainers.Oracle;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public class OracleTests : DatabaseTests, IClassFixture<OracleTestContextFixture>
{
    public OracleTests(OracleTestContextFixture fixture) : base(fixture.DemoContext)
    {
    }

    [Fact]
    public override async Task Deadlock()
    {
        var p1 = DemoContext.Products.Add(new() { Name = "Test1" });
        var p2 = DemoContext.Products.Add(new() { Name = "Test2" });
        await DemoContext.SaveChangesAsync();

        var id1 = p1.Entity.Id;
        var id2 = p2.Entity.Id;

        await using var controlContext = new DemoContext(DemoContext.Options);
        await using var transaction1 = await DemoContext.Database.BeginTransactionAsync();
        await using var transaction2 = await controlContext.Database.BeginTransactionAsync();

        // Each transaction locks one row
        await DemoContext.Products.Where(c => c.Id == id1)
            .ExecuteUpdateAsync(c => c.SetProperty(p => p.Name, "Test11"));
        await controlContext.Products.Where(c => c.Id == id2)
            .ExecuteUpdateAsync(c => c.SetProperty(p => p.Name, "Test21"));

        // Start both cross-updates concurrently to create a deadlock cycle
        var task1 = Task.Run(() => DemoContext.Products
            .Where(c => c.Id == id2)
            .ExecuteUpdateAsync(c => c.SetProperty(p => p.Name, "Test22")));
        var task2 = Task.Run(() => controlContext.Products
            .Where(c => c.Id == id1)
            .ExecuteUpdateAsync(c => c.SetProperty(p => p.Name, "Test12")));

        // Oracle only rolls back the victim's statement, not its transaction,
        // so the non-victim remains blocked. Use WhenAny to catch the victim first.
        var completedTask = await Task.WhenAny(task1, task2);
        await Assert.ThrowsAsync<DeadlockException>(() => completedTask);

        // Roll back the victim's transaction to release its earlier locks
        // and unblock the other session.
        if (completedTask == task1)
        {
            await transaction1.RollbackAsync();
            await task2;
        }
        else
        {
            await transaction2.RollbackAsync();
            await task1;
        }
    }
}

public class OracleTestContextFixture : DemoContextFixture<OracleContainer>
{
    static OracleTestContextFixture()
    {
        Container = new OracleBuilder().Build();
    }

    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString)
        => builder.UseOracle(connectionString).UseExceptionProcessor();

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, string connectionString)
        => builder.UseOracle(connectionString).UseExceptionProcessor();
}
