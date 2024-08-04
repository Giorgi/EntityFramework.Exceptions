using DotNet.Testcontainers.Containers;
using EntityFramework.Exceptions.MySQL.Pomelo;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Threading.Tasks;
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

public class MySqlDemoContextPomeloFixture : DemoContextFixture
{
    private static readonly MySqlContainer MySqlContainer = new MySqlBuilder().Build();

    protected override async Task<DbContextOptionsBuilder<DemoContext>> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder) 
        => builder.UseMySql(await StartAndGetConnection(), new MySqlServerVersion("8.0"), o => o.SchemaBehavior(MySqlSchemaBehavior.Ignore)).UseExceptionProcessor();

    protected override async Task<DbContextOptionsBuilder> BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder) 
        => builder.UseMySql(await StartAndGetConnection(), new MySqlServerVersion("8.0"), o => o.SchemaBehavior(MySqlSchemaBehavior.Ignore)).UseExceptionProcessor();

    private static async Task<string> StartAndGetConnection()
    {
        if (MySqlContainer.State != TestcontainersStates.Running)
        {
            await MySqlContainer.StartAsync();
        }

        return MySqlContainer.GetConnectionString();
    }

    public override Task DisposeAsync()
    {
        return MySqlContainer.DisposeAsync().AsTask();
    }
}