using DotNet.Testcontainers.Containers;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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

    public class PostgreSQLDemoContextFixture : DemoContextFixture
    {
        private static readonly PostgreSqlContainer PostgreSqlContainer = new PostgreSqlBuilder().Build();

        protected override async Task<DbContextOptionsBuilder<DemoContext>> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder) 
            => builder.UseNpgsql(await StartAndGetConnection()).UseExceptionProcessor();

        protected override async Task<DbContextOptionsBuilder> BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder) 
            => builder.UseNpgsql(await StartAndGetConnection()).UseExceptionProcessor();

        private static async Task<string> StartAndGetConnection()
        {
            if (PostgreSqlContainer.State != TestcontainersStates.Running)
            {
                await PostgreSqlContainer.StartAsync();
            }

            return PostgreSqlContainer.GetConnectionString();
        }

        public override Task DisposeAsync() => PostgreSqlContainer.DisposeAsync().AsTask();
    }
}
