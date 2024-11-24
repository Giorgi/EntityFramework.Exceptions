using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFramework.Exceptions.Common;

static class ExceptionFactory
{
    internal static Exception Create<T>(ExceptionProcessorInterceptor<T>.DatabaseError error, Exception exception, IReadOnlyList<EntityEntry> entries) where T : DbException
    {
        if (entries?.Count > 0)
        {
            return error switch
            {
                ExceptionProcessorInterceptor<T>.DatabaseError.CannotInsertNull => new CannotInsertNullException( "Cannot insert null", exception.InnerException, entries),
                ExceptionProcessorInterceptor<T>.DatabaseError.MaxLength => new MaxLengthExceededException( "Maximum length exceeded", exception.InnerException, entries),
                ExceptionProcessorInterceptor<T>.DatabaseError.NumericOverflow => new NumericOverflowException( "Numeric overflow", exception.InnerException, entries),
                ExceptionProcessorInterceptor<T>.DatabaseError.ReferenceConstraint => new ReferenceConstraintException( "Reference constraint violation", exception.InnerException, entries),
                ExceptionProcessorInterceptor<T>.DatabaseError.UniqueConstraint => new UniqueConstraintException( "Unique constraint violation", exception.InnerException, entries),
                _ => null,
            };
        }

        return error switch
        {
            ExceptionProcessorInterceptor<T>.DatabaseError.CannotInsertNull => new CannotInsertNullException("Cannot insert null", exception.InnerException),
            ExceptionProcessorInterceptor<T>.DatabaseError.MaxLength => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException),
            ExceptionProcessorInterceptor<T>.DatabaseError.NumericOverflow => new NumericOverflowException("Numeric overflow", exception.InnerException),
            ExceptionProcessorInterceptor<T>.DatabaseError.ReferenceConstraint => new ReferenceConstraintException("Reference constraint violation", exception.InnerException),
            ExceptionProcessorInterceptor<T>.DatabaseError.UniqueConstraint => new UniqueConstraintException("Unique constraint violation", exception.InnerException),
            _ => null,
        };
    }
}