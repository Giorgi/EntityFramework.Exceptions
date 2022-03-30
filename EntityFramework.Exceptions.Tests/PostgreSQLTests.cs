using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class PostgreSQLTests : DatabaseTests, IClassFixture<PostgreSQLDemoContextFixture>
    {
        public PostgreSQLTests(PostgreSQLDemoContextFixture fixture) : base(fixture.Context)
        {
        }

        [Fact]
        public void DependencyInjectionInterceptorTest()
        {
            using var services = new ServiceCollection()
                .AddExceptionProcessor()
                .BuildServiceProvider()
            ;

            var interceptor = services.GetRequiredService<IInterceptor>();
            Assert.IsType<PostgresExceptionProcessorInterceptor>(interceptor);
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
