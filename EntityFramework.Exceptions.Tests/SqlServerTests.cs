﻿using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public class SqlServerTests : DatabaseTests, IClassFixture<SqlServerDemoContextFixture>
{
    public SqlServerTests(SqlServerDemoContextFixture fixture) : base(fixture.Context)
    {
    }

    public override async Task PrimaryKeyViolationThrowsUniqueConstraintException()
    {
        Context.Database.OpenConnection();

        Context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");
            
        await base.PrimaryKeyViolationThrowsUniqueConstraintException();
            
        Context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF");

        Context.Database.CloseConnection();
    }

    [Fact(Skip = "Skipping as Microsoft.Data.SqlClient throws ArgumentException when numeric value is not in range.")]
    public override Task NumericOverflowViolationThrowsNumericOverflowException()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public void DependencyInjectionInterceptorTest()
    {
        using var services = new ServiceCollection()
            .AddExceptionProcessor()
            .BuildServiceProvider()
        ;

        var interceptor = services.GetRequiredService<IInterceptor>();
        Assert.IsType<SqlServerExceptionProcessorInterceptor>(interceptor);
    }
}

public class SqlServerDemoContextFixture : DemoContextFixture
{
    protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
    {
        return builder.UseSqlServer(configuration.GetConnectionString("SqlServer")).UseExceptionProcessor();
    }
}