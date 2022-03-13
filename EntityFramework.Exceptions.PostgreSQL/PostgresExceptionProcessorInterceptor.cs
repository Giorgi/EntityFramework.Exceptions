using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EntityFramework.Exceptions.PostgreSQL
{
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
        public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        {
            self.AddInterceptors(new PostgresExceptionProcessorInterceptor());
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            self.AddInterceptors(new PostgresExceptionProcessorInterceptor());
            return self;
        }
    }
}
