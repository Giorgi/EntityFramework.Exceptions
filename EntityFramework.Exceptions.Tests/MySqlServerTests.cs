using EntityFramework.Exceptions.MySQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

//[Collection("MySQL Test Collection")]
//public class MySQLServerTests : DatabaseTests, IClassFixture<MySQLDemoContextFixture>
//{
//    public MySQLServerTests(MySQLDemoContextFixture fixture) : base(fixture.Context)
//    {

//    }
//}

public class MySQLDemoContextFixture : DemoContextFixture
{
    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, IConfigurationRoot configuration)
    {
        return builder.UseMySQL(configuration.GetConnectionString("MySQL")).UseExceptionProcessor();
    }
}