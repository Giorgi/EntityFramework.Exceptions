using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.Oracle;

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self
            .AddInterceptors(new OracleExceptionProcessorInterceptor())
            .AddInterceptors(new OracleExceptionProcessorDbCommandInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self)
        where TContext : DbContext
    {
        return self
            .AddInterceptors(new OracleExceptionProcessorInterceptor())
            .AddInterceptors(new OracleExceptionProcessorDbCommandInterceptor());
    }
}