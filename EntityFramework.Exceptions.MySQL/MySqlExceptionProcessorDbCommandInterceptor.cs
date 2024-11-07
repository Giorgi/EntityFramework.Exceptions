using System;
using EntityFramework.Exceptions.Common;

#if POMELO
using MySqlConnector;
namespace EntityFramework.Exceptions.MySQL.Pomelo;
#else
using MySql.Data.MySqlClient;
namespace EntityFramework.Exceptions.MySQL;
#endif

class MySqlExceptionProcessorDbCommandInterceptor : ExceptionProcessorDbCommandInterceptor<MySqlException>
{
    protected override DatabaseError? GetDatabaseError(MySqlException dbException)
    {

#if POMELO
            return dbException.ErrorCode switch
#else
        return (MySqlErrorCode)dbException.Number switch
#endif
        {
            MySqlErrorCode.ColumnCannotBeNull => DatabaseError.CannotInsertNull,
            MySqlErrorCode.DuplicateKeyEntry=> DatabaseError.UniqueConstraint,
            MySqlErrorCode.WarningDataOutOfRange => DatabaseError.NumericOverflow,
            MySqlErrorCode.DataTooLong => DatabaseError.MaxLength,
            MySqlErrorCode.NoReferencedRow => DatabaseError.ReferenceConstraint,
            MySqlErrorCode.RowIsReferenced => DatabaseError.ReferenceConstraint,
            MySqlErrorCode.NoReferencedRow2 => DatabaseError.ReferenceConstraint,
            MySqlErrorCode.RowIsReferenced2 => DatabaseError.ReferenceConstraint,
            _ => null
        };
    }
}