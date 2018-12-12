using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("EntityFramework.Exceptions.MySQL")]
[assembly: InternalsVisibleTo("EntityFramework.Exceptions.SqlServer")]
[assembly: InternalsVisibleTo("EntityFramework.Exceptions.PostgreSQL")]


namespace EntityFramework.Exceptions.Common
{
    public abstract class ExceptionProcessorContextBase<T> : DbContext where T : Exception
    {
        private static readonly Dictionary<DatabaseError, Func<DbUpdateException, Exception>> ExceptionMapping = new Dictionary<DatabaseError, Func<DbUpdateException, Exception>>
        {
            {DatabaseError.MaxLength, exception => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException) },
            {DatabaseError.UniqueConstraint, exception => new UniqueConstraintException("Unique constraint violation", exception.InnerException) },
            {DatabaseError.CannotInsertNull, exception => new CannotInsertNullException("Cannot insert null", exception.InnerException) },
            {DatabaseError.NumericOverflow, exception => new NumericOverflowException("Numeric overflow", exception.InnerException) }
        };

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is T dbException && GetDatabaseError(dbException) is DatabaseError error && ExceptionMapping.TryGetValue(error, out var ctor))
                {
                    throw ctor(ex);
                }

                throw;
            }
        }

        internal abstract DatabaseError? GetDatabaseError(T dbException);
    }

    internal enum DatabaseError
    {
        UniqueConstraint,
        CannotInsertNull,
        MaxLength,
        NumericOverflow
    }
}
