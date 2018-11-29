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

    public class MaxLengthException : Exception
    {
        public MaxLengthException()
        {
        }

        protected MaxLengthException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MaxLengthException(string message) : base(message)
        {
        }

        public MaxLengthException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}