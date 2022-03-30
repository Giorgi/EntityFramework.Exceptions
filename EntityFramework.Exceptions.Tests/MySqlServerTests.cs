using EntityFramework.Exceptions.MySQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

[Collection("MySQL Test Collection")]
public class MySQLServerTests : DatabaseTests, IClassFixture<MySQLDemoContextFixture>
{
    public MySQLServerTests(MySQLDemoContextFixture fixture) : base(fixture.Context)
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
        Assert.IsType<MySqlExceptionProcessorInterceptor>(interceptor);
    }
}

public class MySQLDemoContextFixture : DemoContextFixture
{
    protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
    {
        return builder.UseMySQL(configuration.GetConnectionString("MySQL")).UseExceptionProcessor();
    }
}