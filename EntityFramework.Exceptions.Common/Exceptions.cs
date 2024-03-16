using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFramework.Exceptions.Common;

public class UniqueConstraintException : DbUpdateException
{
    public UniqueConstraintException()
    {
    }

    public UniqueConstraintException(string message) : base(message)
    {
    }

    public UniqueConstraintException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public UniqueConstraintException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    {
    }

    public UniqueConstraintException(string message, Exception innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    {
    }

    public string ConstraintName { get; internal set; }
}

public class CannotInsertNullException : DbUpdateException
{
    public CannotInsertNullException()
    {
    }

    public CannotInsertNullException(string message) : base(message)
    {
    }

    public CannotInsertNullException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CannotInsertNullException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    {
    }

    public CannotInsertNullException(string message, Exception innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    {
    }
}

public class MaxLengthExceededException : DbUpdateException
{
    public MaxLengthExceededException()
    {
    }

    public MaxLengthExceededException(string message) : base(message)
    {
    }

    public MaxLengthExceededException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public MaxLengthExceededException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    {
    }

    public MaxLengthExceededException(string message, Exception innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    {
    }
}

public class NumericOverflowException : DbUpdateException
{
    public NumericOverflowException()
    {
    }

    public NumericOverflowException(string message) : base(message)
    {
    }

    public NumericOverflowException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public NumericOverflowException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    {
    }

    public NumericOverflowException(string message, Exception innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    {
    }
}

public class ReferenceConstraintException : DbUpdateException
{
    public ReferenceConstraintException()
    {
    }

    public ReferenceConstraintException(string message) : base(message)
    {
    }

    public ReferenceConstraintException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ReferenceConstraintException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    {
    }

    public ReferenceConstraintException(string message, Exception innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    {
    }
}