using System.Threading.Tasks;
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
    
#if BULK_OPERATIONS
    [Fact(Skip = "Skipping until ORA-01407 is supported")]
    public override Task RequiredColumnViolationThrowsCannotInsertNullExceptionThroughExecuteUpdate()
    {
        return Task.CompletedTask;
    }
#endif
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