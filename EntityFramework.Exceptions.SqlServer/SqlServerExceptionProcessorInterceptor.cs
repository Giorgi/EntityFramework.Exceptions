using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.SqlServer;

class SqlServerExceptionProcessorInterceptor: ExceptionProcessorInterceptor<SqlException>
{
    private const int ReferenceConstraint = 547;
    private const int CannotInsertNull = 515;
    private const int CannotInsertDuplicateKeyUniqueIndex = 2601;
    private const int CannotInsertDuplicateKeyUniqueConstraint = 2627;
    private const int ArithmeticOverflow = 8115;
    private const int StringOrBinaryDataWouldBeTruncated = 8152;
    
    //SQL Server 2019 added a new error with better error message: https://docs.microsoft.com/en-us/archive/blogs/sql_server_team/string-or-binary-data-would-be-truncated-replacing-the-infamous-error-8152
    private const int StringOrBinaryDataWouldBeTruncated2019 = 2628;
    
    protected override DatabaseError? GetDatabaseError(SqlException dbException)
    {
        return dbException.Number switch
        {
            ReferenceConstraint => DatabaseError.ReferenceConstraint,
            CannotInsertNull => DatabaseError.CannotInsertNull,
            CannotInsertDuplicateKeyUniqueIndex => DatabaseError.UniqueConstraint,
            CannotInsertDuplicateKeyUniqueConstraint => DatabaseError.UniqueConstraint,
            ArithmeticOverflow => DatabaseError.NumericOverflow,
            StringOrBinaryDataWouldBeTruncated => DatabaseError.MaxLength,
            StringOrBinaryDataWouldBeTruncated2019 => DatabaseError.MaxLength,
            _ => null
        };
    }
}

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self) 
        => self.AddInterceptors(new SqlServerExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext 
        => self.AddInterceptors(new SqlServerExceptionProcessorInterceptor());
}