using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    internal sealed class DemoContextWithIdentityInsert : DemoContext
    {
        private readonly string _columnName;

        public DemoContextWithIdentityInsert(DbContextOptions options, string columnName) : base(options)
        {
            _columnName = columnName;
            // need to open the connection for identity insert to work, cf: https://stackoverflow.com/a/58847404
            Database.OpenConnection();
            SetIdentityInsert(true);
        }

        public override void Dispose()
        {
            SetIdentityInsert(false);
            Database.CloseConnection();
            base.Dispose();
        }

        private void SetIdentityInsert(bool enable)
        {
            var onOff = enable ? "ON" : "OFF";
            Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {_columnName} {onOff}");
        }
    }

    public class SqlServerTests : DatabaseTests, IClassFixture<SqlServerDemoContextFixture>, IDisposable
    {
        private bool _enableIdentityInsert;
        private string _identityInsertTableName;

        public SqlServerTests(SqlServerDemoContextFixture fixture) : base(fixture.ContextOptions)
        {
        }

        protected override DemoContext GetNewContext()
        {
            return _enableIdentityInsert
                ? new DemoContextWithIdentityInsert(base.ContextOptions, _identityInsertTableName)
                : base.GetNewContext();
        }

        public override void PrimaryKeyViolationThrowsUniqueConstraintException()
        {
            // IDENTITY_INSERT must be set to ON to write IDs directly in db
            _enableIdentityInsert = true;
            _identityInsertTableName = nameof(DemoContext.Products);

            base.PrimaryKeyViolationThrowsUniqueConstraintException();
        }
    }

    public class SqlServerDemoContextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(
            DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            return builder.UseSqlServer(configuration.GetConnectionString("SqlServer")).UseExceptionProcessor();
        }
    }
}