using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EntityFramework.Exceptions.Oracle;
using Xunit;

namespace EntityFramework.Exceptions.Tests
{
    public class OracleTests : DatabaseTests, IClassFixture<OracleTestContextFixture>
    {
        public OracleTests(OracleTestContextFixture fixture) : base(fixture.Context)
        {
        }

        [Fact]
        public async Task DeleteOfProductInSaleShouldThrowReferenceConstraintException()
        {
            var author = new Author
            {
                Name = "William Shakespeare"
            };
            var book = new Book
            {
                Name = "Hamlet",
                Author = author
            };
            Context.Books.Add(book);
            await Context.SaveChangesAsync();
            CleanupContext();

            var newAuthor = Context.Authors.Find(author.Id);
            Context.Authors.Remove(newAuthor);
            Assert.Throws<ReferenceConstraintException>(() => Context.SaveChanges());
            await Assert.ThrowsAsync<ReferenceConstraintException>(() => Context.SaveChangesAsync());
        }

        private void CleanupContext()
        {
            foreach (var entityEntry in Context.ChangeTracker.Entries())
            {
                entityEntry.State = EntityState.Detached;
            }
        }
    }

    public class OracleTestContextFixture : DemoContextFixture
    {
        protected override DbContextOptionsBuilder<DemoContext> BuildOptions(
            DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
        {
            var connectionString = configuration.GetConnectionString("Oracle");
            return builder.UseOracle(connectionString).UseExceptionProcessor();
        }
    }
}