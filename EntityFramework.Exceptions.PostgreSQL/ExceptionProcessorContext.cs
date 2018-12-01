using EntityFramework.Exceptions.Common;
using Npgsql;

namespace EntityFramework.Exceptions.PostgreSQL
{
    public class ExceptionProcessorContext : ExceptionProcessorContextBase<PostgresException>
    {
        internal override DatabaseError? GetDatabaseError(PostgresException dbException)
        {
            switch (dbException.SqlState)
            {
                //Use https://github.com/npgsql/npgsql/blob/dev/src/Npgsql/PostgresErrorCodes.cs when new version of Npgsql is released.
                case "22001":
                    return DatabaseError.MaxLength;
                case "22003":
                    return DatabaseError.NumericOverflow;
                case "23502":
                    return DatabaseError.CannotInsertNull;
                case "23505":
                    return DatabaseError.UniqueConstraint;
                default:
                    return null;
            }
        }
    }
}
