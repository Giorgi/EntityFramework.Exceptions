using Microsoft.Data.SqlClient;

namespace EntityFramework.Exceptions.SqlServer;

public static class SqlExceptionHelper
{
    private const int ReferenceConstraint = 547; 
    private const int CannotInsertNull = 515;
    private const int CannotInsertDuplicateKeyUniqueIndex = 2601;
    private const int CannotInsertDuplicateKeyUniqueConstraint = 2627;
    private const int ArithmeticOverflow = 8115;
    private const int StringOrBinaryDataWouldBeTruncated = 8152;

    //SQL Server 2019 added a new error with better error message: https://docs.microsoft.com/en-us/archive/blogs/sql_server_team/string-or-binary-data-would-be-truncated-replacing-the-infamous-error-8152
    private const int StringOrBinaryDataWouldBeTruncated2019 = 2628;

    public static bool IsReferenceConstraintError(this SqlException exception) => exception.Number == ReferenceConstraint;
    public static bool IsCannotInsertNullError(this SqlException exception) => exception.Number == CannotInsertNull;
    public static bool IsNumericOverflowError(this SqlException exception) => exception.Number == ArithmeticOverflow;
    public static bool IsUniqueConstraintError(this SqlException exception) => exception.Number is CannotInsertDuplicateKeyUniqueConstraint or CannotInsertDuplicateKeyUniqueIndex;
    public static bool IsMaxLengthExceededError(this SqlException exception) => exception.Number is StringOrBinaryDataWouldBeTruncated or StringOrBinaryDataWouldBeTruncated2019;
}