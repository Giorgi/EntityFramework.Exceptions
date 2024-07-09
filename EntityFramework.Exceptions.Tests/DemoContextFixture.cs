using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using EntityFramework.Exceptions.Tests.ConstraintTests;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.EntityFrameworkCore.Extensions;

namespace EntityFramework.Exceptions.Tests;

public abstract class DemoContextFixture : IDisposable
{
    internal DemoContext DemoContext { get; }
    internal SameNameIndexesContext SameNameIndexesContext {get; }

    protected DemoContextFixture()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.{environment}.json", optional: true).Build();
            
        var demoContextOptions = BuildDemoContextOptions(new DbContextOptionsBuilder<DemoContext>(), configuration).Options;

        DemoContext = new DemoContext(demoContextOptions);
        DemoContext.Database.EnsureCreated();

        var sameNameIndexesContextOptions = BuildSameNameIndexesContextOptions(new DbContextOptionsBuilder<SameNameIndexesContext>(), configuration).Options;

        SameNameIndexesContext = new SameNameIndexesContext(sameNameIndexesContextOptions);
        
        var isMySql = MySqlDatabaseFacadeExtensions.IsMySql(SameNameIndexesContext.Database) || MySQLDatabaseFacadeExtensions.IsMySql(SameNameIndexesContext.Database);
        var isSqlite = SameNameIndexesContext.Database.IsSqlite();

        if (!(isMySql || isSqlite))
        {
            var relationalDatabaseCreator = SameNameIndexesContext.Database.GetService<IRelationalDatabaseCreator>();
            var generateCreateScript = relationalDatabaseCreator.GenerateCreateScript();
            relationalDatabaseCreator.CreateTables();
        }
    }

    protected abstract DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration);

    protected virtual DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, IConfigurationRoot configuration) => builder;

    public void Dispose()
    {
        DemoContext.Database.EnsureDeleted();
    }
}