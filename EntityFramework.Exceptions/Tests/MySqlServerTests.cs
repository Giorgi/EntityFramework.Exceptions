using EntityFramework.Exceptions.MySQL;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

[Collection("MySql Test Collection")]
public class MySqlServerTests : DatabaseTests, IClassFixture<MySqlDemoContextFixture>
{
    public MySqlServerTests(MySqlDemoContextFixture fixture) : base(fixture.DemoContext)
    {

    }
}

public class MySqlDemoContextFixture : DemoContextFixture<MySqlContainer>
{
    public MySqlDemoContextFixture()
    {
        Container = new MySqlBuilder("mysql:8.0").Build();
    }

    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString)
        => builder.UseMySQL(connectionString).UseExceptionProcessor();

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(
        DbContextOptionsBuilder builder, string connectionString)
        => builder.UseMySQL(connectionString).UseExceptionProcessor();
}
