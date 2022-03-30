using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EntityFramework.Exceptions.Oracle;
using Xunit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.Exceptions.Tests;

public class OracleTests : DatabaseTests, IClassFixture<OracleTestContextFixture>
{
    public OracleTests(OracleTestContextFixture fixture) : base(fixture.Context)
    {
    }

    [Fact]
    public void DependencyInjectionInterceptorTest()
    {
        using var services = new ServiceCollection()
            .AddExceptionProcessor()
            .BuildServiceProvider()
        ;

        var interceptor = services.GetRequiredService<IInterceptor>();
        Assert.IsType<OracleExceptionProcessorInterceptor>(interceptor);
    }
}

public class OracleTestContextFixture : DemoContextFixture
{
    protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("Oracle");
        return builder.UseOracle(connectionString).UseExceptionProcessor();
    }
}