using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EntityFramework.Exceptions.Oracle;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public class OracleTests : DatabaseTests, IClassFixture<OracleTestContextFixture>
{
    public OracleTests(OracleTestContextFixture fixture) : base(fixture.DemoContext, fixture.SameNameIndexesContext)
    {
    }
}

public class OracleTestContextFixture : DemoContextFixture
{
    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
    {
        return builder.UseOracle(Environment.GetEnvironmentVariable("Oracle")).UseExceptionProcessor();
    }

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, IConfigurationRoot configuration)
    {
        return builder.UseOracle(Environment.GetEnvironmentVariable("Oracle")).UseExceptionProcessor();
    }
}