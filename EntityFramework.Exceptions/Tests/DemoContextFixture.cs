using DotNet.Testcontainers.Containers;
using EntityFramework.Exceptions.Tests.ConstraintTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.EntityFrameworkCore.Extensions;
using System.Threading.Tasks;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public abstract class DemoContextFixture<T> : IAsyncLifetime where T : IContainer
{
    public static T Container { get; internal set; }

    internal DemoContext DemoContext { get; private set; }
    internal SameNameIndexesContext SameNameIndexesContext { get; private set; }

    protected abstract DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString);

    protected virtual DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, string connectionString) => builder;

    public async Task InitializeAsync()
    {
        var connectionString = "";

        if (Container is not null && Container.State != TestcontainersStates.Running)
        {
            await Container.StartAsync();
            connectionString = (Container as IDatabaseContainer)?.GetConnectionString();
        }

        var optionsBuilder = BuildDemoContextOptions(new DbContextOptionsBuilder<DemoContext>(), connectionString);
        DemoContext = new DemoContext(optionsBuilder.Options);
        DemoContext.Database.EnsureCreated();

        var sameNameIndexesOptionsBuilder = BuildSameNameIndexesContextOptions(new DbContextOptionsBuilder<SameNameIndexesContext>(), connectionString);
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

    public virtual Task DisposeAsync() => Container != null ? Container.DisposeAsync().AsTask() : Task.CompletedTask;
}