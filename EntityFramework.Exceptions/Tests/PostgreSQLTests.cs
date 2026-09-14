using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class PostgreSQLTests : DatabaseTests, IClassFixture<PostgreSQLDemoContextFixture>
    {
        public PostgreSQLTests(PostgreSQLDemoContextFixture fixture) : base(fixture.DemoContext, fixture.SameNameIndexesContext)
        {
        }
    }

    public class PostgreSQLDemoContextFixture : DemoContextFixture<PostgreSqlContainer>
    {
        static PostgreSQLDemoContextFixture()
        {
            Container = new PostgreSqlBuilder().WithImage("postgres:18").Build();
        }

        protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString)
            => builder.UseNpgsql(connectionString).UseExceptionProcessor();

        protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, string connectionString)
            => builder.UseNpgsql(connectionString).UseExceptionProcessor();
    }
}
