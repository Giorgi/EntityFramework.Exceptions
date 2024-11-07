using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.SqlServer;

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self
            .AddInterceptors(new SqlServerExceptionProcessorInterceptor())
            .AddInterceptors(new SqlServerExceptionProcessorDbCommandInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
    {
        return self
            .AddInterceptors(new SqlServerExceptionProcessorInterceptor())
            .AddInterceptors(new SqlServerExceptionProcessorDbCommandInterceptor());
    }
}