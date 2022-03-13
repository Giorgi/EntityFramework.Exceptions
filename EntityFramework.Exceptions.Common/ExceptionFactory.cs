using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFramework.Exceptions.Common
{
    static class ExceptionFactory
    {
        internal static Exception Create<T>(ExceptionProcessorInterceptor<T>.DatabaseError error, DbUpdateException exception, IReadOnlyList<EntityEntry> entries) where T : DbException
        {
            return error switch
            {
                ExceptionProcessorInterceptor<T>.DatabaseError.CannotInsertNull when entries.Count > 0 => new CannotInsertNullException("Cannot insert null", exception.InnerException, entries),
                ExceptionProcessorInterceptor<T>.DatabaseError.CannotInsertNull when entries.Count == 0 => new CannotInsertNullException("Cannot insert null", exception.InnerException),
                ExceptionProcessorInterceptor<T>.DatabaseError.MaxLength when entries.Count > 0 => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException, entries),
                ExceptionProcessorInterceptor<T>.DatabaseError.MaxLength when entries.Count == 0 => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException),
                ExceptionProcessorInterceptor<T>.DatabaseError.NumericOverflow when entries.Count > 0 => new NumericOverflowException("Numeric overflow", exception.InnerException, entries),
                ExceptionProcessorInterceptor<T>.DatabaseError.NumericOverflow when entries.Count == 0 => new NumericOverflowException("Numeric overflow", exception.InnerException),
                ExceptionProcessorInterceptor<T>.DatabaseError.ReferenceConstraint when entries.Count > 0 => new ReferenceConstraintException("Reference constraint violation", exception.InnerException, entries),
                ExceptionProcessorInterceptor<T>.DatabaseError.ReferenceConstraint when entries.Count == 0 => new ReferenceConstraintException("Reference constraint violation", exception.InnerException),
                ExceptionProcessorInterceptor<T>.DatabaseError.UniqueConstraint when entries.Count > 0 => new UniqueConstraintException("Unique constraint violation", exception.InnerException, entries),
                ExceptionProcessorInterceptor<T>.DatabaseError.UniqueConstraint when entries.Count == 0 => new UniqueConstraintException("Unique constraint violation", exception.InnerException),
                _ => null,
            };
        }
    }
}