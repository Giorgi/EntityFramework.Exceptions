using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFramework.Exceptions.Common
{
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

        public UniqueConstraintException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
        {
        }

        public UniqueConstraintException(string message, Exception innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
        {
        }

        public UniqueConstraintException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
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

        public CannotInsertNullException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
        {
        }

        public CannotInsertNullException(string message, Exception innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
        {
        }

        public CannotInsertNullException(SerializationInfo info, StreamingContext context) : base(info, context)
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

        public MaxLengthExceededException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
        {
        }

        public MaxLengthExceededException(string message, Exception innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
        {
        }

        public MaxLengthExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
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

        public NumericOverflowException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
        {
        }

        public NumericOverflowException(string message, Exception innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
        {
        }

        public NumericOverflowException(SerializationInfo info, StreamingContext context) : base(info, context)
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

        public ReferenceConstraintException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
        {
        }

        public ReferenceConstraintException(string message, Exception innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
        {
        }

        public ReferenceConstraintException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}