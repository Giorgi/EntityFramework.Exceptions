using EntityFramework.Exceptions.Tests.ConstraintTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.EntityFrameworkCore.Extensions;
using System.Threading.Tasks;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public abstract class DemoContextFixture : IAsyncLifetime
{
    internal DemoContext DemoContext { get; private set; }
    internal SameNameIndexesContext SameNameIndexesContext {get; private set; }

    protected abstract Task<DbContextOptionsBuilder<DemoContext>> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder);

    protected virtual Task<DbContextOptionsBuilder> BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder) => Task.FromResult(builder);
    
    public async Task InitializeAsync()
    {
        var optionsBuilder = await BuildDemoContextOptions(new DbContextOptionsBuilder<DemoContext>());
        DemoContext = new DemoContext(optionsBuilder.Options);
        DemoContext.Database.EnsureCreated();

        var sameNameIndexesOptionsBuilder = await BuildSameNameIndexesContextOptions(new DbContextOptionsBuilder<SameNameIndexesContext>());
        SameNameIndexesContext = new SameNameIndexesContext(sameNameIndexesOptionsBuilder.Options);

        var isMySql = MySqlDatabaseFacadeExtensions.IsMySql(SameNameIndexesContext.Database) || MySQLDatabaseFacadeExtensions.IsMySql(SameNameIndexesContext.Database);
        var isSqlite = SameNameIndexesContext.Database.IsSqlite();
        var isOracle = SameNameIndexesContext.Database.IsOracle();

        if (!(isMySql || isSqlite || isOracle))
        {
            var relationalDatabaseCreator = SameNameIndexesContext.Database.GetService<IRelationalDatabaseCreator>();
            relationalDatabaseCreator.CreateTables();
        }
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}