using EntityFramework.Exceptions.MySQL.Pomelo;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Testcontainers.MySql;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

[Collection("MySql Test Collection")]
public class MySqlServerPomeloTests : DatabaseTests, IClassFixture<MySqlDemoContextPomeloFixture>
{
    public MySqlServerPomeloTests(MySqlDemoContextPomeloFixture fixture) : base(fixture.DemoContext)
    {

    }
}

public class MySqlDemoContextPomeloFixture : DemoContextFixture<MySqlContainer>
{
    static MySqlDemoContextPomeloFixture()
    {
        Container = new MySqlBuilder().Build();
    }

    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString) 
        => builder.UseMySql(connectionString, new MySqlServerVersion("8.0"), o => o.SchemaBehavior(MySqlSchemaBehavior.Ignore)).UseExceptionProcessor();

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(
        DbContextOptionsBuilder builder, string connectionString) 
        => builder.UseMySql(connectionString, new MySqlServerVersion("8.0"), o => o.SchemaBehavior(MySqlSchemaBehavior.Ignore)).UseExceptionProcessor();

}