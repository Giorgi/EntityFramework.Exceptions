using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EntityFramework.Exceptions.PostgreSQL
{
    class PostgresExceptionProcessorInterceptor : ExceptionProcessorInterceptor<PostgresException>
    {
        protected override DatabaseError? GetDatabaseError(PostgresException dbException)
        {
            switch (dbException.SqlState)
            {
                case PostgresErrorCodes.StringDataRightTruncation:
                    return DatabaseError.MaxLength;
                case PostgresErrorCodes.NumericValueOutOfRange:
                    return DatabaseError.NumericOverflow;
                case PostgresErrorCodes.NotNullViolation:
                    return DatabaseError.CannotInsertNull;
                case PostgresErrorCodes.UniqueViolation:
                    return DatabaseError.UniqueConstraint;
                case PostgresErrorCodes.ForeignKeyViolation:
                    return DatabaseError.ReferenceConstraint;
                default:
                    return null;
            }
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
