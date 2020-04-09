using EntityFramework.Exceptions.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using static SQLitePCL.raw;

namespace EntityFramework.Exceptions.Sqlite
{
    class SqliteExceptionProcessorStateManager : ExceptionProcessorStateManager<SqliteException>
    {
        public SqliteExceptionProcessorStateManager(StateManagerDependencies dependencies) : base(dependencies)
        {
        }

        protected override DatabaseError? GetDatabaseError(SqliteException dbException)
        {
            if (dbException.SqliteErrorCode == SQLITE_TOOBIG)
            {
                return DatabaseError.MaxLength;
            }

            if (dbException.SqliteErrorCode == SQLITE_CONSTRAINT)
            {
                switch (dbException.SqliteExtendedErrorCode)
                {
                    case SQLITE_CONSTRAINT_NOTNULL:
                        return DatabaseError.CannotInsertNull;
                    case SQLITE_CONSTRAINT_UNIQUE:
                        return DatabaseError.UniqueConstraint;
                    case SQLITE_CONSTRAINT_FOREIGNKEY:
                        return DatabaseError.ReferenceConstraint;
                }
            }

            return null;
        }
    }

    public static class ExceptionProcessorExtensions
    {
        public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        {
            self.ReplaceService<IStateManager, SqliteExceptionProcessorStateManager>();
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(
            this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            self.ReplaceService<IStateManager, SqliteExceptionProcessorStateManager>();
            return self;
        }
    }
}