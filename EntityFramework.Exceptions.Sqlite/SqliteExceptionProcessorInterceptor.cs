using EntityFramework.Exceptions.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using static SQLitePCL.raw;

namespace EntityFramework.Exceptions.Sqlite
{
    class SqliteExceptionProcessorInterceptor : ExceptionProcessorInterceptor<SqliteException>
    {
        protected override DatabaseError? GetDatabaseError(SqliteException dbException)
        {
            if (dbException.SqliteErrorCode == SQLITE_CONSTRAINT || dbException.SqliteErrorCode == SQLITE_TOOBIG)
            {
                switch (dbException.SqliteExtendedErrorCode)
                {
                    case SQLITE_TOOBIG:
                        return DatabaseError.MaxLength;
                    case SQLITE_CONSTRAINT_NOTNULL:
                        return DatabaseError.CannotInsertNull;
                    case SQLITE_CONSTRAINT_UNIQUE:
                    case SQLITE_CONSTRAINT_PRIMARYKEY:
                        return DatabaseError.UniqueConstraint;
                    case SQLITE_CONSTRAINT_FOREIGNKEY:
                        return DatabaseError.ReferenceConstraint;
                    default:
                        return null;
                }
            }

            return null;
        }
    }

    public static class ExceptionProcessorExtensions
    {
        public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        {
            self.AddInterceptors(new SqliteExceptionProcessorInterceptor());
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            self.AddInterceptors(new SqliteExceptionProcessorInterceptor());
            return self;
        }
    }
}