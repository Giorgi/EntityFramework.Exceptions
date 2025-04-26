using DbExceptionClassifier.Common;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using static SQLitePCL.raw;

namespace DbExceptionClassifier.Sqlite;

public class SqliteExceptionClassifier : IDbExceptionClassifier
{
    public bool IsReferenceConstraintError(DbException exception) => exception is SqliteException { SqliteExtendedErrorCode: SQLITE_CONSTRAINT_FOREIGNKEY };

    public bool IsCannotInsertNullError(DbException exception) => exception is SqliteException { SqliteExtendedErrorCode: SQLITE_CONSTRAINT_NOTNULL };

    public bool IsNumericOverflowError(DbException exception) => false;

    public bool IsUniqueConstraintError(DbException exception) => exception is SqliteException
    {
        SqliteExtendedErrorCode: SQLITE_CONSTRAINT_UNIQUE or SQLITE_CONSTRAINT_PRIMARYKEY
    };

    public bool IsMaxLengthExceededError(DbException exception) => exception is SqliteException { SqliteExtendedErrorCode: SQLITE_TOOBIG };
}