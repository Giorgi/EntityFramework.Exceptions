using DbExceptionClassifier.PostgreSQL;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EntityFramework.Exceptions.PostgreSQL;

class PostgresExceptionProcessorInterceptor()
    : ExceptionProcessorInterceptor<PostgresException>(new PostgreSQLExceptionClassifier());

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        => self.AddInterceptors(new PostgresExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        => self.AddInterceptors(new PostgresExceptionProcessorInterceptor());
}