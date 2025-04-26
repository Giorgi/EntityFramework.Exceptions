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

public class SqlServerDemoContextFixture : DemoContextFixture<MsSqlContainer>
{
    static SqlServerDemoContextFixture()
    {
        Container = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest").Build();
    }

    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString) 
        => builder.UseSqlServer(connectionString).UseExceptionProcessor();

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, string connectionString) => 
        builder.UseSqlServer(connectionString).UseExceptionProcessor();
}