using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace EntityFramework.Exceptions.PostgreSQL;

class PostgresExceptionProcessorInterceptor : ExceptionProcessorInterceptor<PostgresException>
{
    protected override DatabaseError? GetDatabaseError(PostgresException dbException)
    {
        return dbException.SqlState switch
        {
            PostgresErrorCodes.StringDataRightTruncation => DatabaseError.MaxLength,
            PostgresErrorCodes.NumericValueOutOfRange => DatabaseError.NumericOverflow,
            PostgresErrorCodes.NotNullViolation => DatabaseError.CannotInsertNull,
            PostgresErrorCodes.UniqueViolation => DatabaseError.UniqueConstraint,
            PostgresErrorCodes.ForeignKeyViolation => DatabaseError.ReferenceConstraint,
            _ => null
        };
    }
}

public static class ExceptionProcessorExtensions
{
    public static IServiceCollection AddExceptionProcessor(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
	{
        services.Add(ServiceDescriptor.Describe(typeof(IInterceptor), typeof(PostgresExceptionProcessorInterceptor), serviceLifetime));
        return services;
    }

    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self.AddInterceptors(new PostgresExceptionProcessorInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
    {
        return self.AddInterceptors(new PostgresExceptionProcessorInterceptor());
    }
}