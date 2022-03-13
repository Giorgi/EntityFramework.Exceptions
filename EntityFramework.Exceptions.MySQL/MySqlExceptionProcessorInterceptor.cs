using System;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

#if POMELO
using MySqlConnector;
namespace EntityFramework.Exceptions.MySQL.Pomelo
#else
using MySql.Data.MySqlClient;
namespace EntityFramework.Exceptions.MySQL
#endif
{
    class MySqlExceptionProcessorInterceptor : ExceptionProcessorInterceptor<MySqlException>
    {
        private const int NoReferencedRow = 1216;
        private const int RowIsReferenced = 1217;
        private const int RowIsReferenced2 = 1451;
        private const int NoReferencedRow2 = 1452;
        private const int ColumnCannotBeNull = 1048;
        private const int DuplicateEntryForKey = 1062;
        private const int OutOfRangeValueForColumn = 1264;
        private const int DataTooLongForColumn = 1406;
        
        protected override DatabaseError? GetDatabaseError(MySqlException dbException)
        {
            return dbException.Number switch
            {
                ColumnCannotBeNull => DatabaseError.CannotInsertNull,
                DuplicateEntryForKey => DatabaseError.UniqueConstraint,
                OutOfRangeValueForColumn => DatabaseError.NumericOverflow,
                DataTooLongForColumn => DatabaseError.MaxLength,
                NoReferencedRow => DatabaseError.ReferenceConstraint,
                RowIsReferenced => DatabaseError.ReferenceConstraint,
                NoReferencedRow2 => DatabaseError.ReferenceConstraint,
                RowIsReferenced2 => DatabaseError.ReferenceConstraint,
                _ => null
            };
        }
    }

    public static class ExceptionProcessorExtensions
    {
        public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        {
            self.AddInterceptors(new MySqlExceptionProcessorInterceptor());
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            self.AddInterceptors(new MySqlExceptionProcessorInterceptor());
            return self;
        }
    }
}
