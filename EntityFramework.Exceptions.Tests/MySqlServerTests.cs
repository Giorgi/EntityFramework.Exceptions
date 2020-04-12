using EntityFramework.Exceptions.MySQL.Pomelo;
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
    }

    public class MySQLDemoContextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            return builder.UseMySql(configuration.GetConnectionString("MySQL")).UseExceptionProcessor();
        }
    }
}
