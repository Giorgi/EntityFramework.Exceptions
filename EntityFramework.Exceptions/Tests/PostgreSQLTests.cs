using EntityFramework.Exceptions.Common;
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

        [Fact]
        public async Task MaxLengthViolationThrowsMaxLengthExceededExceptionThroughRawSql()
        {
            var longName = new string('G', DemoContext.ProductNameMaxLength + 5);

            Assert.Throws<MaxLengthExceededException>(() =>
                DemoContext.Database.ExecuteSqlInterpolated(
                    $"INSERT INTO \"Products\" (\"Name\") VALUES ({longName})"));
            await Assert.ThrowsAsync<MaxLengthExceededException>(() =>
                DemoContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO \"Products\" (\"Name\") VALUES ({longName})"));
        }
    }

    public class PostgreSQLDemoContextFixture : DemoContextFixture<PostgreSqlContainer>
    {
        static PostgreSQLDemoContextFixture()
        {
            Container = new PostgreSqlBuilder().Build();
        }

        protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString)
            => builder.UseNpgsql(connectionString).UseExceptionProcessor();

        protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, string connectionString)
            => builder.UseNpgsql(connectionString).UseExceptionProcessor();
    }
}
