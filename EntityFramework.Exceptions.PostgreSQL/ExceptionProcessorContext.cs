using Npgsql;

namespace EntityFramework.Exceptions.PostgreSQL
{
    public class ExceptionProcessorContext : ExceptionProcessorContextBase<PostgresException>
    {
        internal override DatabaseError? GetDatabaseError(PostgresException postgresException)
        {
            switch (postgresException.SqlState)
            {
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
