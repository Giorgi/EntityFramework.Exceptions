using DbExceptionClassifier.Sqlite;
using EntityFramework.Exceptions.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.Sqlite;

class SqliteExceptionProcessorInterceptor()
    : ExceptionProcessorInterceptor<SqliteException>(new SqliteExceptionClassifier());

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        => self.AddInterceptors(new SqliteExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        => self.AddInterceptors(new SqliteExceptionProcessorInterceptor());
}
