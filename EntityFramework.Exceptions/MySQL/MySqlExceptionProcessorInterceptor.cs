using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

#if POMELO
using MySqlConnector;
using DbExceptionClassifier.MySQL.Pomelo;
namespace EntityFramework.Exceptions.MySQL.Pomelo;
#else
using MySql.Data.MySqlClient;
using DbExceptionClassifier.MySQL;
namespace EntityFramework.Exceptions.MySQL;
#endif

class MySqlExceptionProcessorInterceptor() : ExceptionProcessorInterceptor<MySqlException>(new MySQLExceptionClassifier());

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self) 
        => self.AddInterceptors(new MySqlExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext 
        => self.AddInterceptors(new MySqlExceptionProcessorInterceptor());
}