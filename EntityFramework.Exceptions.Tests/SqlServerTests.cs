using DotNet.Testcontainers.Containers;
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
}

public class SqlServerDemoContextFixture : DemoContextFixture
{
    static readonly MsSqlContainer MsSqlContainer = new MsSqlBuilder().Build();

    protected override async Task<DbContextOptionsBuilder<DemoContext>> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder) 
        => builder.UseSqlServer(await StartAndGetConnection()).UseExceptionProcessor();

    protected override async Task<DbContextOptionsBuilder> BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder) => 
        builder.UseSqlServer(await StartAndGetConnection()).UseExceptionProcessor();

    public override Task DisposeAsync()
    {
        return MsSqlContainer.DisposeAsync().AsTask();
    }

    private static async Task<string> StartAndGetConnection()
    {
        if (MsSqlContainer.State != TestcontainersStates.Running)
        {
            await MsSqlContainer.StartAsync();
        }

        return MsSqlContainer.GetConnectionString();
    }
}