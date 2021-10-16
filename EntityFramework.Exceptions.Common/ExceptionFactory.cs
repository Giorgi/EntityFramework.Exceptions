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
            switch (error)
            {
                case ExceptionProcessorInterceptor<T>.DatabaseError.CannotInsertNull when entries.Count > 0:
                    return new CannotInsertNullException("Cannot insert null", exception.InnerException, entries);
                case ExceptionProcessorInterceptor<T>.DatabaseError.CannotInsertNull when entries.Count == 0:
                    return new CannotInsertNullException("Cannot insert null", exception.InnerException);

                case ExceptionProcessorInterceptor<T>.DatabaseError.MaxLength when entries.Count > 0:
                    return new MaxLengthExceededException("Maximum length exceeded", exception.InnerException, entries);
                case ExceptionProcessorInterceptor<T>.DatabaseError.MaxLength when entries.Count == 0:
                    return new MaxLengthExceededException("Maximum length exceeded", exception.InnerException);

                case ExceptionProcessorInterceptor<T>.DatabaseError.NumericOverflow when entries.Count > 0:
                    return new NumericOverflowException("Numeric overflow", exception.InnerException, entries);
                case ExceptionProcessorInterceptor<T>.DatabaseError.NumericOverflow when entries.Count == 0:
                    return new NumericOverflowException("Numeric overflow", exception.InnerException);

                case ExceptionProcessorInterceptor<T>.DatabaseError.ReferenceConstraint when entries.Count > 0:
                    return new ReferenceConstraintException("Reference constraint violation", exception.InnerException, entries);
                case ExceptionProcessorInterceptor<T>.DatabaseError.ReferenceConstraint when entries.Count == 0:
                    return new ReferenceConstraintException("Reference constraint violation", exception.InnerException);

                case ExceptionProcessorInterceptor<T>.DatabaseError.UniqueConstraint when entries.Count > 0:
                    return new UniqueConstraintException("Unique constraint violation", exception.InnerException, entries);
                case ExceptionProcessorInterceptor<T>.DatabaseError.UniqueConstraint when entries.Count == 0:
                    return new UniqueConstraintException("Unique constraint violation", exception.InnerException);

                default:
                    return null;
            }
        }
    }
}