using System;
using System.Runtime.Serialization;

namespace EntityFramework.Exceptions
{
    public class UniqueConstraintException : Exception
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

        protected UniqueConstraintException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class CannotInsertNullException : Exception
    {
        public CannotInsertNullException()
        {
        }

        protected CannotInsertNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CannotInsertNullException(string message) : base(message)
        {
        }

        public CannotInsertNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class MaxLengthExceededException : Exception
    {
        public MaxLengthExceededException()
        {
        }

        protected MaxLengthExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MaxLengthExceededException(string message) : base(message)
        {
        }

        public MaxLengthExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class NumericOverflowException : Exception
    {
        public NumericOverflowException()
        {
        }

        protected NumericOverflowException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NumericOverflowException(string message) : base(message)
        {
        }

        public NumericOverflowException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}