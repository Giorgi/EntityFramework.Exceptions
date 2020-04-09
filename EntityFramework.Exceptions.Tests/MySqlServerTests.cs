using EntityFramework.Exceptions.MySQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class MySQLServerTests : DatabaseTests, IClassFixture<MySQLDemoContextFixture>, IDisposable
    {
        public MySQLServerTests(MySQLDemoContextFixture fixture) : base(fixture.Context)
        {
        }

        [Fact(Skip = "Skipping until EF Core 3.1 is supported by MySQL")]
        public override void UniqueColumnViolationThrowsUniqueConstraintException()
        {
        }

        [Fact(Skip = "Skipping until EF Core 3.1 is supported by MySQL")]
        public override void RequiredColumnViolationThrowsCannotInsertNullException()
        {
        }

        [Fact(Skip = "Skipping until EF Core 3.1 is supported by MySQL")]
        public override void MaxLengthViolationThrowsMaxLengthExceededException()
        {
        }

        [Fact(Skip = "Skipping until EF Core 3.1 is supported by MySQL")]
        public override void NumericOverflowViolationThrowsNumericOverflowException()
        {
        }

        [Fact(Skip = "Skipping until EF Core 3.1 is supported by MySQL")]
        public override void ReferenceViolationThrowsReferenceConstraintException()
        {
        }
    }

    public class MySQLDemoContextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            return builder.UseMySQL(configuration.GetConnectionString("MySQL")).UseExceptionProcessor();
        }
    }
}
