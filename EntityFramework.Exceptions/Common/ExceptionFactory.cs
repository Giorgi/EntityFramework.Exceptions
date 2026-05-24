using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFramework.Exceptions.Common;

static class ExceptionFactory
{
    internal static Exception Create<T>(ExceptionProcessorInterceptor<T>.DatabaseError error, Exception exception, IReadOnlyList<EntityEntry> entries) where T : DbException
    {
        return error switch
        {
            ExceptionProcessorInterceptor<T>.DatabaseError.CannotInsertNull => new CannotInsertNullException("Cannot insert null", exception, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.MaxLength => new MaxLengthExceededException("Maximum length exceeded", exception, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.NumericOverflow => new NumericOverflowException("Numeric overflow", exception, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.ReferenceConstraint => new ReferenceConstraintException("Reference constraint violation", exception, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.UniqueConstraint => new UniqueConstraintException("Unique constraint violation", exception, entries),
            ExceptionProcessorInterceptor<T>.DatabaseError.Deadlock => new DeadlockException("Deadlock", exception, entries),
            _ => null,
        };
    }
}
