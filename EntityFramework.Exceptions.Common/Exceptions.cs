using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFramework.Exceptions.Common
{
    public class UniqueConstraintException : DbUpdateException
    {
        public UniqueConstraintException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UniqueConstraintException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
        {
        }

        public UniqueConstraintException(string message, Exception innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
        {
        }
    }

    public class CannotInsertNullException : DbUpdateException
    {
        public CannotInsertNullException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CannotInsertNullException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
        {
        }

        public CannotInsertNullException(string message, Exception innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
        {
        }
    }

    public class MaxLengthExceededException : DbUpdateException
    {
        public MaxLengthExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MaxLengthExceededException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
        {
        }

        public MaxLengthExceededException(string message, Exception innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
        {
        }
    }

    public class NumericOverflowException : DbUpdateException
    {
        public NumericOverflowException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NumericOverflowException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
        {
        }

        public NumericOverflowException(string message, Exception innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
        {
        }
    }
}