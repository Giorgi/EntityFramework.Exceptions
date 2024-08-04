using DotNet.Testcontainers.Containers;
using EntityFramework.Exceptions.Oracle;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Testcontainers.Oracle;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public class OracleTests : DatabaseTests, IClassFixture<OracleTestContextFixture>
{
    public OracleTests(OracleTestContextFixture fixture) : base(fixture.DemoContext)
    {
    }
}

public class OracleTestContextFixture : DemoContextFixture
{
    private static readonly OracleContainer OracleContainer = new OracleBuilder().Build();

    protected override async Task<DbContextOptionsBuilder<DemoContext>> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder) 
        => builder.UseOracle(await StartAndGetConnection()).UseExceptionProcessor();

    protected override async Task<DbContextOptionsBuilder> BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder) 
        => builder.UseOracle(await StartAndGetConnection()).UseExceptionProcessor();

    private static async Task<string> StartAndGetConnection()
    {
        if (OracleContainer.State != TestcontainersStates.Running)
        {
            await OracleContainer.StartAsync();
        }

        return OracleContainer.GetConnectionString();
    }

    public override Task DisposeAsync()
    {
        return OracleContainer.DisposeAsync().AsTask();
    }
}