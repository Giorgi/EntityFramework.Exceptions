using EntityFramework.Exceptions.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using static SQLitePCL.raw;

namespace EntityFramework.Exceptions.Sqlite;

class SqliteExceptionProcessorDbCommandInterceptor : ExceptionProcessorDbCommandInterceptor<SqliteException>
{
    protected override DatabaseError? GetDatabaseError(SqliteException dbException)
    {
        if (dbException.SqliteErrorCode == SQLITE_CONSTRAINT || dbException.SqliteErrorCode == SQLITE_TOOBIG)
        {
            return dbException.SqliteExtendedErrorCode switch
            {
                SQLITE_TOOBIG => DatabaseError.MaxLength,
                SQLITE_CONSTRAINT_NOTNULL => DatabaseError.CannotInsertNull,
                SQLITE_CONSTRAINT_UNIQUE => DatabaseError.UniqueConstraint,
                SQLITE_CONSTRAINT_PRIMARYKEY => DatabaseError.UniqueConstraint,
                SQLITE_CONSTRAINT_FOREIGNKEY => DatabaseError.ReferenceConstraint,
                _ => null
            };
        }

        return null;
    }
}