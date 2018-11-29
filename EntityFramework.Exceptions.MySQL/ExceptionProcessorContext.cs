using MySql.Data.MySqlClient;

namespace EntityFramework.Exceptions.MySQL
{
    public class ExceptionProcessorContext : ExceptionProcessorContextBase<MySqlException>
    {
        private const int ColumnCannotBeNull = 1048;
        private const int DuplicateEntryForKey = 1062;
        private const int OutOfRangeValueForColumn = 1264;
        private const int DataTooLongForColumn = 1406;

        internal override DatabaseError? GetDatabaseError(MySqlException dbException)
        {
            switch (dbException.Number)
            {
                case ColumnCannotBeNull:
                    return DatabaseError.CannotInsertNull;
                case DuplicateEntryForKey:
                    return DatabaseError.UniqueConstraint;
                case OutOfRangeValueForColumn:
                    return DatabaseError.NumericOverflow;
                case DataTooLongForColumn:
                    return DatabaseError.MaxLength;
                default:
                    return null;
            }
        }
    }
}
