using DbExceptionClassifier.Common;
using Npgsql;
using System.Data.Common;

namespace DbExceptionClassifier.PostgreSQL;

public class PostgreSQLExceptionClassifier : IDbExceptionClassifier
{
    public bool IsReferenceConstraintError(DbException exception) => exception is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation };
    public bool IsCannotInsertNullError(DbException exception) => exception is PostgresException { SqlState: PostgresErrorCodes.NotNullViolation };
    public bool IsNumericOverflowError(DbException exception) => exception is PostgresException { SqlState: PostgresErrorCodes.NumericValueOutOfRange };
    public bool IsUniqueConstraintError(DbException exception) => exception is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    public bool IsMaxLengthExceededError(DbException exception) => exception is PostgresException { SqlState: PostgresErrorCodes.StringDataRightTruncation };
}