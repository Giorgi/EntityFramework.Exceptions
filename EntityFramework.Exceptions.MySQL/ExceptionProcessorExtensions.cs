using Microsoft.EntityFrameworkCore;

#if POMELO
namespace EntityFramework.Exceptions.MySQL.Pomelo;
#else
namespace EntityFramework.Exceptions.MySQL;
#endif

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self
            .AddInterceptors(new MySqlExceptionProcessorInterceptor())
            .AddInterceptors(new MySqlExceptionProcessorDbCommandInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
    {
        return self
            .AddInterceptors(new MySqlExceptionProcessorInterceptor())
            .AddInterceptors(new MySqlExceptionProcessorDbCommandInterceptor());
    }
}