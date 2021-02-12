using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace EntityFramework.Exceptions.Common
{
    static class ExceptionFactory
    {
        internal static Exception Create<T>(ExceptionProcessorStateManager<T>.DatabaseError error, DbUpdateException exception, List<InternalEntityEntry> entries) where T : DbException
        {
            switch (error)
            {
                case ExceptionProcessorStateManager<T>.DatabaseError.CannotInsertNull when entries.Count > 0:
                    return new CannotInsertNullException("Cannot insert null", exception.InnerException, entries);
                case ExceptionProcessorStateManager<T>.DatabaseError.CannotInsertNull when entries.Count == 0:
                    return new CannotInsertNullException("Cannot insert null", exception.InnerException);

                case ExceptionProcessorStateManager<T>.DatabaseError.MaxLength when entries.Count > 0:
                    return new MaxLengthExceededException("Maximum length exceeded", exception.InnerException, entries);
                case ExceptionProcessorStateManager<T>.DatabaseError.MaxLength when entries.Count == 0:
                    return new MaxLengthExceededException("Maximum length exceeded", exception.InnerException);

                case ExceptionProcessorStateManager<T>.DatabaseError.NumericOverflow when entries.Count > 0:
                    return new NumericOverflowException("Numeric overflow", exception.InnerException, entries);
                case ExceptionProcessorStateManager<T>.DatabaseError.NumericOverflow when entries.Count == 0:
                    return new NumericOverflowException("Numeric overflow", exception.InnerException);

                case ExceptionProcessorStateManager<T>.DatabaseError.ReferenceConstraint when entries.Count > 0:
                    return new ReferenceConstraintException("Reference constraint violation", exception.InnerException, entries);
                case ExceptionProcessorStateManager<T>.DatabaseError.ReferenceConstraint when entries.Count == 0:
                    return new ReferenceConstraintException("Reference constraint violation", exception.InnerException);

                case ExceptionProcessorStateManager<T>.DatabaseError.UniqueConstraint when entries.Count > 0:
                    return new UniqueConstraintException("Unique constraint violation", exception.InnerException, entries);
                case ExceptionProcessorStateManager<T>.DatabaseError.UniqueConstraint when entries.Count == 0:
                    return new UniqueConstraintException("Unique constraint violation", exception.InnerException);

                default:
                    return null;
            }
        }
    }
}