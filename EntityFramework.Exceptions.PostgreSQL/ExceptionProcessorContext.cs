using System;
using Npgsql;

namespace EntityFramework.Exceptions.PostgreSQL
{
    public class ExceptionProcessorContext : ExceptionProcessorContextBase<PostgresException>
    {
        internal override DatabaseError? GetDatabaseError(PostgresException postgresException)
        {
            if (postgresException == null)
            {
                return null;
            }

            switch (postgresException.SqlState)
            {
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
