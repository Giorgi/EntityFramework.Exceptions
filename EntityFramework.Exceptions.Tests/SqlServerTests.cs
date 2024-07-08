using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Tests.ConstraintTests;
using Microsoft.Data.SqlClient;
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
    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
    {
        return builder.UseSqlServer(configuration.GetConnectionString("SqlServer")).UseExceptionProcessor();
    }

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, IConfigurationRoot configuration)
    {
        return builder.UseSqlServer(configuration.GetConnectionString("SqlServer")).UseExceptionProcessor();
    }
}