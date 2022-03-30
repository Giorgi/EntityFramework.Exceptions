using EntityFramework.Exceptions.MySQL.Pomelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

[Collection("MySQL Test Collection")]
public class MySQLServerPomeloTests : DatabaseTests, IClassFixture<MySQLDemoContextPomeloFixture>
{
    public MySQLServerPomeloTests(MySQLDemoContextPomeloFixture fixture) : base(fixture.Context)
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

public class MySQLDemoContextPomeloFixture : DemoContextFixture
{
    protected override DbContextOptionsBuilder<DemoContext> BuildOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
    {
        return builder.UseMySql(configuration.GetConnectionString("MySQL"), new MySqlServerVersion("5.7")).UseExceptionProcessor();
    }
}