using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.PostgreSQL;

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self
            .AddInterceptors(new PostgresExceptionProcessorInterceptor())
            .AddInterceptors(new PostgresExceptionProcessorDbCommandInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
    {
        return self
            .AddInterceptors(new PostgresExceptionProcessorInterceptor())
            .AddInterceptors(new PostgresExceptionProcessorDbCommandInterceptor());
    }
}