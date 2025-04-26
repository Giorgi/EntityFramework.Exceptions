using System.Data.Common;
using DbExceptionClassifier.Common;

#if POMELO
using MySqlConnector;
namespace DbExceptionClassifier.MySQL.Pomelo;
#else
using MySql.Data.MySqlClient;
namespace DbExceptionClassifier.MySQL;
#endif


public class MySQLExceptionClassifier : IDbExceptionClassifier
{
    private static MySqlErrorCode GetErrorCode(DbException dbException)
    {
        if (dbException is not MySqlException mySqlException)
        {
            return MySqlErrorCode.None;
        }

#if POMELO
        return mySqlException.ErrorCode;
#else
        return (MySqlErrorCode)mySqlException.Number;
#endif
    }

    public bool IsReferenceConstraintError(DbException exception)
    {
        var errorCode = GetErrorCode(exception);

        return errorCode is MySqlErrorCode.NoReferencedRow or
                            MySqlErrorCode.RowIsReferenced or
                            MySqlErrorCode.NoReferencedRow2 or
                            MySqlErrorCode.RowIsReferenced2;
    }

    public bool IsCannotInsertNullError(DbException exception)
    {
        var errorCode = GetErrorCode(exception);
        return errorCode == MySqlErrorCode.ColumnCannotBeNull;
    }

    public bool IsNumericOverflowError(DbException exception)
    {
        var errorCode = GetErrorCode(exception);
        return errorCode == MySqlErrorCode.WarningDataOutOfRange;
    }

    public bool IsUniqueConstraintError(DbException exception)
    {
        var errorCode = GetErrorCode(exception);
        return errorCode == MySqlErrorCode.DuplicateKeyEntry;
    }

    public bool IsMaxLengthExceededError(DbException exception)
    {
        var errorCode = GetErrorCode(exception);
        return errorCode == MySqlErrorCode.DataTooLong;
    }
}