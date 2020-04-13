using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class SqlServerTests : DatabaseTests, IClassFixture<SqlServerDemoContextFixture>, IDisposable
    {
        public SqlServerTests(SqlServerDemoContextFixture fixture) : base(fixture.Context)
        {
        }

        public override async Task PrimaryKeyViolationThrowsUniqueConstraintException()
        {
            Context.Database.OpenConnection();

            Context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");
            
            await base.PrimaryKeyViolationThrowsUniqueConstraintException();
            
            Context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF");

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
