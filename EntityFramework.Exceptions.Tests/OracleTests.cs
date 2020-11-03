using System;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EntityFramework.Exceptions.Oracle;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    class OracleTests : DatabaseTests, IClassFixture<OracleTestContextFixture>
    {
        OracleTests(OracleTestContextFixture fixture) : base(fixture.Context)
        {
        }
    }
    public class OracleTestContextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(
            DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            var connectionString = configuration.GetConnectionString("Oracle");
            return builder.UseOracle(connectionString).UseExceptionProcessor();
        }
    }
}