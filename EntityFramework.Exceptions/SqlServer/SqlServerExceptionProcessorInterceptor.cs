using DbExceptionClassifier.SqlServer;
using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.SqlServer;

class SqlServerExceptionProcessorInterceptor() 
    : ExceptionProcessorInterceptor<SqlException>(new SqlServerExceptionClassifier())
{
}

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self) 
        => self.AddInterceptors(new SqlServerExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext 
        => self.AddInterceptors(new SqlServerExceptionProcessorInterceptor());
}