using EntityFramework.Exceptions.MySQL.Pomelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

[Collection("MySQL Test Collection")]
public class MySQLServerPomeloTests : DatabaseTests, IClassFixture<MySQLDemoContextPomeloFixture>
{
    public MySQLServerPomeloTests(MySQLDemoContextPomeloFixture fixture) : base(fixture.DemoContext, fixture.SameNameIndexesContext)
    {

    }
}

public class MySQLDemoContextPomeloFixture : DemoContextFixture
{
    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
    {
        return builder.UseMySql(configuration.GetConnectionString("MySQL"), new MySqlServerVersion("5.7"), o => o.SchemaBehavior(MySqlSchemaBehavior.Ignore)).UseExceptionProcessor();
    }

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, IConfigurationRoot configuration)
    {
        return builder.UseMySql(configuration.GetConnectionString("MySQL"), new MySqlServerVersion("5.7"), o => o.SchemaBehavior(MySqlSchemaBehavior.Ignore)).UseExceptionProcessor();
    }
}