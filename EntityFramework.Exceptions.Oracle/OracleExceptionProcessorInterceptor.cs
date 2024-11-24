using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace EntityFramework.Exceptions.Oracle;

class OracleExceptionProcessorInterceptor : ExceptionProcessorInterceptor<OracleException>
{
    private const int CannotInsertNull = 1400;
    private const int UniqueConstraintViolation = 1;
    private const int IntegrityConstraintViolation = 2291;
    private const int ChildRecordFound = 2292;
    private const int NumericOverflow = 1438;
    private const int NumericOrValueError = 12899;
        
    protected override DatabaseError? GetDatabaseError(OracleException dbException)
    {
        return dbException.Number switch
        {
            IntegrityConstraintViolation => DatabaseError.ReferenceConstraint,
            ChildRecordFound => DatabaseError.ReferenceConstraint,
            CannotInsertNull => DatabaseError.CannotInsertNull,
            NumericOrValueError => DatabaseError.MaxLength,
            NumericOverflow => DatabaseError.NumericOverflow,
            UniqueConstraintViolation => DatabaseError.UniqueConstraint,
            _ => null
        };
    }
}
    
public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self.AddInterceptors(new OracleExceptionProcessorInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self)
        where TContext : DbContext
    {
        return self.AddInterceptors(new OracleExceptionProcessorInterceptor());
    }
}