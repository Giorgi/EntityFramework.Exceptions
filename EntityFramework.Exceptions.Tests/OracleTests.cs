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
    // TODO support ORA-01407: cannot update (string) to NULL 
    [Fact(Skip = "Oracle has differing error codes for inserting null and updating to null")]
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