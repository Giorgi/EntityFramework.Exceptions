using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFramework.Exceptions.Common;

static class ExceptionFactory
{
    internal static Exception Create(DatabaseError error, DbUpdateException exception, IReadOnlyList<EntityEntry> entries)
    {
        return error switch
        {
            DatabaseError.CannotInsertNull when entries.Count > 0 => new CannotInsertNullException("Cannot insert null", exception.InnerException, entries),
            DatabaseError.CannotInsertNull when entries.Count == 0 => new CannotInsertNullException("Cannot insert null", exception.InnerException),
            DatabaseError.MaxLength when entries.Count > 0 => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException, entries),
            DatabaseError.MaxLength when entries.Count == 0 => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException),
            DatabaseError.NumericOverflow when entries.Count > 0 => new NumericOverflowException("Numeric overflow", exception.InnerException, entries),
            DatabaseError.NumericOverflow when entries.Count == 0 => new NumericOverflowException("Numeric overflow", exception.InnerException),
            DatabaseError.ReferenceConstraint when entries.Count > 0 => new ReferenceConstraintException("Reference constraint violation", exception.InnerException, entries),
            DatabaseError.ReferenceConstraint when entries.Count == 0 => new ReferenceConstraintException("Reference constraint violation", exception.InnerException),
            DatabaseError.UniqueConstraint when entries.Count > 0 => new UniqueConstraintException("Unique constraint violation", exception.InnerException, entries),
            DatabaseError.UniqueConstraint when entries.Count == 0 => new UniqueConstraintException("Unique constraint violation", exception.InnerException),
            _ => null,
        };
    }
}