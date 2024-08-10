using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.SqlServer;

internal class SqlServerExceptionProcessorInterceptor: ExceptionProcessorInterceptor<SqlException>
{
    protected override DatabaseError? GetDatabaseError(SqlException dbException)
    {
        if (dbException.IsMaxLengthExceededError()) return DatabaseError.MaxLength;
        if (dbException.IsNumericOverflowError()) return DatabaseError.NumericOverflow;
        if (dbException.IsCannotInsertNullError()) return DatabaseError.CannotInsertNull;
        if (dbException.IsUniqueConstraintError()) return DatabaseError.UniqueConstraint;
        if (dbException.IsReferenceConstraintError()) return DatabaseError.ReferenceConstraint;

        return null;
    }
}

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self.AddInterceptors(new SqlServerExceptionProcessorInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
    {
        return self.AddInterceptors(new SqlServerExceptionProcessorInterceptor());
    }
}