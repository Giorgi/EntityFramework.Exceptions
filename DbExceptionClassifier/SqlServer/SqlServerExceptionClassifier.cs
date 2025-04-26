using DbExceptionClassifier.Common;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace DbExceptionClassifier.SqlServer;

public class SqlServerExceptionClassifier : IDbExceptionClassifier
{
    private const int ReferenceConstraint = 547;
    private const int CannotInsertNull = 515;
    private const int CannotInsertDuplicateKeyUniqueIndex = 2601;
    private const int CannotInsertDuplicateKeyUniqueConstraint = 2627;
    private const int ArithmeticOverflow = 8115;
    private const int StringOrBinaryDataWouldBeTruncated = 8152;

    //SQL Server 2019 added a new error with better error message: https://docs.microsoft.com/en-us/archive/blogs/sql_server_team/string-or-binary-data-would-be-truncated-replacing-the-infamous-error-8152
    private const int StringOrBinaryDataWouldBeTruncated2019 = 2628;

    public bool IsReferenceConstraintError(DbException exception) => exception is SqlException { Number: ReferenceConstraint };
    public bool IsCannotInsertNullError(DbException exception) => exception is SqlException { Number: CannotInsertNull };
    public bool IsNumericOverflowError(DbException exception) => exception is SqlException { Number: ArithmeticOverflow };
    public bool IsUniqueConstraintError(DbException exception) => exception is SqlException { Number: CannotInsertDuplicateKeyUniqueConstraint or CannotInsertDuplicateKeyUniqueIndex };
    public bool IsMaxLengthExceededError(DbException exception) => exception is SqlException { Number: StringOrBinaryDataWouldBeTruncated or StringOrBinaryDataWouldBeTruncated2019 };
}