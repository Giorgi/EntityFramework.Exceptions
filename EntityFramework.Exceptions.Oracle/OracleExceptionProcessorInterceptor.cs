using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace EntityFramework.Exceptions.Oracle
{
    public class OracleExceptionProcessorInterceptor : ExceptionProcessorInterceptor<OracleException>
    {
        private const int CannotInsertNull = 1400;
        private const int UniqueConstraintViolation = 1;
        private const int IntegrityConstraintViolation = 2291;
        private const int ChildRecordFound = 2292;
        private const int NumericOverflow = 1438;
        private const int NumericOrValueError = 12899;
        
        protected override DatabaseError? GetDatabaseError(OracleException dbException)
        {
            switch (dbException.Number)
            {
                case IntegrityConstraintViolation: 
                case ChildRecordFound:
                    return DatabaseError.ReferenceConstraint;
                case CannotInsertNull:
                    return DatabaseError.CannotInsertNull;
                case NumericOrValueError:
                    return DatabaseError.MaxLength;
                case NumericOverflow:
                    return DatabaseError.NumericOverflow;
                case UniqueConstraintViolation:
                    return DatabaseError.UniqueConstraint;
                default:
                    return null;
            }
        }
    }
    
    public static class ExceptionProcessorExtensions
    {
        public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        {
            self.AddInterceptors(new OracleExceptionProcessorInterceptor());
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self)
            where TContext : DbContext
        {
            self.AddInterceptors(new OracleExceptionProcessorInterceptor());
            return self;
        }
    }
}