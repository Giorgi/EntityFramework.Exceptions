using EntityFramework.Exceptions.Common;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public class SqlServerTests : DatabaseTests, IClassFixture<SqlServerDemoContextFixture>
{
    public SqlServerTests(SqlServerDemoContextFixture fixture) : base(fixture.DemoContext, fixture.SameNameIndexesContext)
    {
    }

    [Fact]
    public override async Task PrimaryKeyViolationThrowsUniqueConstraintException()
    {
        DemoContext.Database.OpenConnection();

        DemoContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");

        await base.PrimaryKeyViolationThrowsUniqueConstraintException();

        DemoContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF");

        DemoContext.Database.CloseConnection();
    }

    [Fact(Skip = "Skipping as Microsoft.Data.SqlClient throws ArgumentException when numeric value is not in range.")]
    public override Task NumericOverflowViolationThrowsNumericOverflowException()
    {
        return Task.CompletedTask;
    }

    [Fact(Skip = "Skipping as Microsoft.Data.SqlClient throws ArgumentException when numeric value is not in range.")]
    public override Task NumericOverflowViolationThrowsNumericOverflowExceptionThroughExecuteUpdate()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task SaveChangesDeadlockThrowsDeadlockException()
    {
        var seed1 = DemoContext.Products.Add(new() { Name = "DLSave1" });
        var seed2 = DemoContext.Products.Add(new() { Name = "DLSave2" });
        await DemoContext.SaveChangesAsync();

        var id1 = seed1.Entity.Id;
        var id2 = seed2.Entity.Id;

        DemoContext.ChangeTracker.Clear();

        await using var controlContext = new DemoContext(DemoContext.Options);
        await using var tx1 = await DemoContext.Database.BeginTransactionAsync();
        await using var tx2 = await controlContext.Database.BeginTransactionAsync();

        var p1Lock = new Product { Id = id1, Name = "DLSave1a" };
        DemoContext.Attach(p1Lock).Property(p => p.Name).IsModified = true;
        await DemoContext.SaveChangesAsync();

        var p2Lock = new Product { Id = id2, Name = "DLSave2a" };
        controlContext.Attach(p2Lock).Property(p => p.Name).IsModified = true;
        await controlContext.SaveChangesAsync();

        var p2Cross = new Product { Id = id2, Name = "DLSave2b" };
        DemoContext.Attach(p2Cross).Property(p => p.Name).IsModified = true;

        var p1Cross = new Product { Id = id1, Name = "DLSave1b" };
        controlContext.Attach(p1Cross).Property(p => p.Name).IsModified = true;

        await Assert.ThrowsAsync<DeadlockException>(() => Task.WhenAll(
            Task.Run(() => DemoContext.SaveChangesAsync()),
            Task.Run(() => controlContext.SaveChangesAsync())));
    }
}

public class SqlServerDemoContextFixture : DemoContextFixture<MsSqlContainer>
{
    private static readonly ExceptionProcessorOptions Options = new() { HandleDeadlock = true };

    static SqlServerDemoContextFixture()
    {
        Container = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest").Build();
    }

    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString)
        => builder.UseSqlServer(connectionString).UseExceptionProcessor(Options);

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, string connectionString) =>
        builder.UseSqlServer(connectionString).UseExceptionProcessor(Options);
}