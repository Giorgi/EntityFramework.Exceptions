using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Npgsql;

namespace EntityFramework.Exceptions.PostgreSQL
{
    class PostgresExceptionProcessorStateManager : ExceptionProcessorStateManager<PostgresException>
    {
        public PostgresExceptionProcessorStateManager(StateManagerDependencies dependencies) : base(dependencies)
        {
        }

        protected override DatabaseError? GetDatabaseError(PostgresException dbException)
        {
            switch (dbException.SqlState)
            {
                //Use https://github.com/npgsql/npgsql/blob/dev/src/Npgsql/PostgresErrorCodes.cs when new version of Npgsql is released.
                case "22001":
                    return DatabaseError.MaxLength;
                case "22003":
                    return DatabaseError.NumericOverflow;
                case "23502":
                    return DatabaseError.CannotInsertNull;
                case "23505":
                    return DatabaseError.UniqueConstraint;
                default:
                    return null;
            }
        }
    }

    public static class ExceptionProcessorExtensions
    {
        public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        {
            self.ReplaceService<IStateManager, PostgresExceptionProcessorStateManager>();
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            self.ReplaceService<IStateManager, PostgresExceptionProcessorStateManager>();
            return self;
        }
    }
}
