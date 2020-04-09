using EntityFramework.Exceptions.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class SqliteTests : DatabaseTests, IClassFixture<SqliteDemoContextFixture>, IDisposable
    {
        public SqliteTests(SqliteDemoContextFixture fixture) : base(fixture.Context)
        {
        }

        [Fact(Skip = "Skipping as SQLite does not enforce max length")]
        public override void MaxLengthViolationThrowsMaxLengthExceededException()
        {
               
        }

        [Fact(Skip = "Skipping as SQLite does not enforce numeric length")]
        public override void NumericOverflowViolationThrowsNumericOverflowException()
        {
            
        }
    }

    public class SqliteDemoContextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            return builder.UseSqlite(configuration.GetConnectionString("Sqlite")).UseExceptionProcessor();
        }
    }
}
