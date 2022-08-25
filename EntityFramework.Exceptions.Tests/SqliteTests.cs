using EntityFramework.Exceptions.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class SqliteTests : DatabaseTests, IClassFixture<SqliteDemoContextFixture>
    {
        private const int SqliteLimitLength = 0;

        public SqliteTests(SqliteDemoContextFixture fixture) : base(fixture.Context)
        {
        }

        [DllImport("e_sqlite3", CallingConvention = CallingConvention.Cdecl,EntryPoint = "sqlite3_limit")]
        private static extern int SetLimit(sqlite3 db, int id, int newVal);

        [Fact(Skip = "Skippping because of https://github.com/dotnet/efcore/issues/27597")]
        public override async Task MaxLengthViolationThrowsMaxLengthExceededException()
        {
            Context.Database.OpenConnection();
            
            var handle = (Context.Database.GetDbConnection() as SqliteConnection).Handle;
            var limit = SetLimit(handle, SqliteLimitLength, DemoContext.ProductNameMaxLength);
            
            await base.MaxLengthViolationThrowsMaxLengthExceededException();

            SetLimit(handle, SqliteLimitLength, limit);

            Context.Database.CloseConnection();
        }

        [Fact(Skip = "Skipping as SQLite does not enforce numeric length")]
        public override Task NumericOverflowViolationThrowsNumericOverflowException()
        {
            return Task.CompletedTask;
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
