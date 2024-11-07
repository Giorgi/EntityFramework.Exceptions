using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.Sqlite;

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self
            .AddInterceptors(new SqliteExceptionProcessorInterceptor())
            .AddInterceptors(new SqliteExceptionProcessorDbCommandInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
    {
        return self
            .AddInterceptors(new SqliteExceptionProcessorInterceptor())
            .AddInterceptors(new SqliteExceptionProcessorDbCommandInterceptor());
    }
}