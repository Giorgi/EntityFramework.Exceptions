using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EntityFramework.Exceptions.Oracle;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class OracleTests : DatabaseTests, IClassFixture<OracleTestsontextFixture>
    {
        public OracleTests(OracleTestsontextFixture fixture) : base(fixture.Context)
        {
        }

    }
    
    public class OracleTestsontextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            //var connectionString = configuration.GetConnectionString("Oracle");
            var connectionString = "Data Source=brio.WORLD;Persist Security Info=True;User ID=bri;Password=bri;";
            return builder.UseOracle(connectionString).UseExceptionProcessor();
        }
    }
}
