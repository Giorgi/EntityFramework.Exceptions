using System.Data.SqlClient;
using EntityFramework.Exceptions.Common;

namespace EntityFramework.Exceptions.SqlServer
{
    public class ExceptionProcessorContext : ExceptionProcessorContextBase<SqlException>
    {
        private const int CannotInsertNull = 515;
        private const int CannotInsertDuplicateKeyUniqueIndex = 2601;
        private const int CannotInsertDuplicateKeyUniqueConstraint = 2627;
        private const int ArithmeticOverflow = 8115;
        private const int StringOrBinaryDataWouldBeTruncated = 8152;

        protected override DatabaseError? GetDatabaseError(SqlException dbException)
        {
            switch (dbException.Number)
            {
                case CannotInsertNull:
                    return DatabaseError.CannotInsertNull;
                case CannotInsertDuplicateKeyUniqueIndex:
                case CannotInsertDuplicateKeyUniqueConstraint:
                    return DatabaseError.UniqueConstraint;
                case ArithmeticOverflow:
                    return DatabaseError.NumericOverflow;
                case StringOrBinaryDataWouldBeTruncated:
                    return DatabaseError.MaxLength;
                default:
                    return null;
            }
        }
    }
}
