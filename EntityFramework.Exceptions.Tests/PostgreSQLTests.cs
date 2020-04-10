using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class PostgreSQLTests : DatabaseTests, IClassFixture<PostgreSQLDemoContextFixture>, IDisposable
    {
        public PostgreSQLTests(PostgreSQLDemoContextFixture fixture): base(fixture.ContextOptions)
        {
        }
    }

    public class PostgreSQLDemoContextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            return builder.UseNpgsql(configuration.GetConnectionString("PostgreSQL")).UseExceptionProcessor();
        }
    }
}
