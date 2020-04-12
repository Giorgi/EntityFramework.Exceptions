using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class SqlServerTests : DatabaseTests, IClassFixture<SqlServerDemoContextFixture>, IDisposable
    {
        public SqlServerTests(SqlServerDemoContextFixture fixture) : base(fixture.Context)
        {
        }

        public override void PrimaryKeyViolationThrowsUniqueConstraintException()
        {
            Context.Database.OpenConnection();

            Context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");
            
            base.PrimaryKeyViolationThrowsUniqueConstraintException();
            
            Context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");

            Context.Database.CloseConnection();
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
