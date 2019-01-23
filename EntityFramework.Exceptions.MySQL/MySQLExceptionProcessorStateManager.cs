using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MySql.Data.MySqlClient;

namespace EntityFramework.Exceptions.MySQL
{
    class MySqlExceptionProcessorStateManager : ExceptionProcessorStateManager<MySqlException>
    {
        private const int ColumnCannotBeNull = 1048;
        private const int DuplicateEntryForKey = 1062;
        private const int OutOfRangeValueForColumn = 1264;
        private const int DataTooLongForColumn = 1406;

        public MySqlExceptionProcessorStateManager(StateManagerDependencies dependencies) : base(dependencies)
        {
        }

        protected override DatabaseError? GetDatabaseError(MySqlException dbException)
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

    public static class ExceptionProcessorExtensions
    {
        public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        {
            self.ReplaceService<IStateManager, MySqlExceptionProcessorStateManager>();
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            self.ReplaceService<IStateManager, MySqlExceptionProcessorStateManager>();
            return self;
        }
    }
}
