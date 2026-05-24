using DbExceptionClassifier.Sqlite;
using EntityFramework.Exceptions.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.Sqlite;

class SqliteExceptionProcessorInterceptor(ExceptionProcessorOptions options = null)
    : ExceptionProcessorInterceptor<SqliteException>(new SqliteExceptionClassifier(), options);

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        => self.AddInterceptors(new SqliteExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self, ExceptionProcessorOptions options)
        => self.AddInterceptors(new SqliteExceptionProcessorInterceptor(options));

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        => self.AddInterceptors(new SqliteExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self, ExceptionProcessorOptions options) where TContext : DbContext
        => self.AddInterceptors(new SqliteExceptionProcessorInterceptor(options));
}
