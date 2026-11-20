using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

// Shared with the MySqlConnector project, which defines MYSQLCONNECTOR.
#if MYSQLCONNECTOR
using MySqlConnector;
using DbExceptionClassifier.MySqlConnector;
namespace EntityFramework.Exceptions.MySqlConnector;

class MySqlExceptionProcessorInterceptor() : ExceptionProcessorInterceptor<MySqlException>(new MySqlConnectorExceptionClassifier());
#else
using MySql.Data.MySqlClient;
using DbExceptionClassifier.MySQL;
namespace EntityFramework.Exceptions.MySQL;

class MySqlExceptionProcessorInterceptor() : ExceptionProcessorInterceptor<MySqlException>(new MySQLExceptionClassifier());
#endif

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        => self.AddInterceptors(new MySqlExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        => self.AddInterceptors(new MySqlExceptionProcessorInterceptor());
}
