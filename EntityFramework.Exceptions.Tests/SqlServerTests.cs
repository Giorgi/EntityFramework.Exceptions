using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class SqlServerTests : DatabaseTests, IClassFixture<SqlServerDemoContextFixture>, IDisposable
    {
        public SqlServerTests(SqlServerDemoContextFixture fixture) : base(fixture.ContextOptions)
        {
        }

        [Fact(Skip = "Skipping as IDENTITY_INSERT must be set to ON to write IDs directly in db")]
        public override void PrimaryKeyViolationThrowsUniqueConstraintException()
        {
        }
    }

    public class SqlServerDemoContextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            return builder.UseSqlServer(configuration.GetConnectionString("SqlServer")).UseExceptionProcessor();
        }
    }
}
