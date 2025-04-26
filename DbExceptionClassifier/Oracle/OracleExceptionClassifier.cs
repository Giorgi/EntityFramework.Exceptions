using System.Data.Common;
using DbExceptionClassifier.Common;
using Oracle.ManagedDataAccess.Client;

namespace DbExceptionClassifier.Oracle;

public class OracleExceptionClassifier : IDbExceptionClassifier
{
    private const int CannotInsertNull = 1400;
    private const int CannotUpdateToNull = 1407;
    private const int UniqueConstraintViolation = 1;
    private const int IntegrityConstraintViolation = 2291;
    private const int ChildRecordFound = 2292;
    private const int NumericOverflow = 1438;
    private const int NumericOrValueError = 12899;

    public bool IsReferenceConstraintError(DbException exception) => exception is OracleException { Number: IntegrityConstraintViolation or ChildRecordFound };

    public bool IsCannotInsertNullError(DbException exception) => exception is OracleException { Number: CannotInsertNull or CannotUpdateToNull };

    public bool IsNumericOverflowError(DbException exception) => exception is OracleException { Number: NumericOverflow };

    public bool IsUniqueConstraintError(DbException exception) => exception is OracleException { Number: UniqueConstraintViolation };

    public bool IsMaxLengthExceededError(DbException exception) => exception is OracleException { Number: NumericOrValueError };
}