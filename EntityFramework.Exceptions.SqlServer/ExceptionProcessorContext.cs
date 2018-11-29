using System.Data.SqlClient;

namespace EntityFramework.Exceptions.SqlServer
{
    public class ExceptionProcessorContext : ExceptionProcessorContextBase<SqlException>
    {
        internal override DatabaseError? GetDatabaseError(SqlException sqlException)
        {
            switch (sqlException.Number)
            {
                case 515:
                    return DatabaseError.CannotInsertNull;
                case 2601:
                case 2627:
                    return DatabaseError.UniqueConstraint;
                case 8152:
                    return DatabaseError.MaxLength;
                default:
                    return null;
            }
        }
    }
}
