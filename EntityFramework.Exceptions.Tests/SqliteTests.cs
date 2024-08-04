using System.IO;
using EntityFramework.Exceptions.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class SqliteTests : DatabaseTests, IClassFixture<SqliteDemoContextFixture>
    {
        private const int SqliteLimitLength = 0;

        public SqliteTests(SqliteDemoContextFixture fixture) : base(fixture.DemoContext)
        {
        }

        [DllImport("e_sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_limit")]
        private static extern int SetLimit(sqlite3 db, int id, int newVal);

        [Fact(Skip = "Skipping because of https://github.com/dotnet/efcore/issues/27597")]
        public override async Task MaxLengthViolationThrowsMaxLengthExceededException()
        {
            DemoContext.Database.OpenConnection();

            var handle = (DemoContext.Database.GetDbConnection() as SqliteConnection).Handle;
            var limit = SetLimit(handle, SqliteLimitLength, DemoContext.ProductNameMaxLength);

            await base.MaxLengthViolationThrowsMaxLengthExceededException();

            SetLimit(handle, SqliteLimitLength, limit);

            DemoContext.Database.CloseConnection();
        }

        [Fact(Skip = "Skipping as SQLite does not enforce numeric length")]
        public override Task NumericOverflowViolationThrowsNumericOverflowException()
        {
            return Task.CompletedTask;
        }
    }

    public class SqliteDemoContextFixture : DemoContextFixture
    {
        private const string ConnectionString = "DataSource=file::memory:?cache=shared";

        protected override Task<DbContextOptionsBuilder<DemoContext>> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder) 
            => Task.FromResult(builder.UseSqlite(ConnectionString).UseExceptionProcessor());

        protected override Task<DbContextOptionsBuilder> BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder) 
            => Task.FromResult(builder.UseSqlite(ConnectionString).UseExceptionProcessor());

        public override Task DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }
}
