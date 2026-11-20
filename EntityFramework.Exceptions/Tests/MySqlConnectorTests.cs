using DbExceptionClassifier.MySqlConnector;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Testcontainers.MySql;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

// Runs the MySQL suite on Microting.EntityFrameworkCore.MySql (the maintained Pomelo fork, built on MySqlConnector)
// through the generic UseExceptionProcessor(IDbExceptionClassifier) overload, as documented in the README.
[Collection("MySqlConnector Test Collection")]
public class MySqlConnectorTests(MySqlConnectorDemoContextFixture fixture)
    : DatabaseTests(fixture.DemoContext), IClassFixture<MySqlConnectorDemoContextFixture>;

public class MySqlConnectorDemoContextFixture : DemoContextFixture<MySqlContainer>
{
    private static readonly MySqlServerVersion ServerVersion = new(new Version(8, 0));

    public MySqlConnectorDemoContextFixture()
    {
        Container = new MySqlBuilder("mysql:8.0").Build();
    }

    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString)
        => builder.UseMySql(connectionString, ServerVersion).UseExceptionProcessor(new MySqlConnectorExceptionClassifier());

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, string connectionString)
        => builder.UseMySql(connectionString, ServerVersion).UseExceptionProcessor(new MySqlConnectorExceptionClassifier());
}

static class MySqlDatabaseFacadeTestExtensions
{
    // Both MySql.EntityFrameworkCore and MySqlConnector-based providers define an IsMySql() extension, so calling
    // either is ambiguous here. Their provider names both contain "MySql".
    public static bool IsAnyMySql(this DatabaseFacade database)
        => database.ProviderName?.Contains("MySql", StringComparison.OrdinalIgnoreCase) == true;
}
