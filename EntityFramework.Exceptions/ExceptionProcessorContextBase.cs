using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("EntityFramework.Exceptions.SqlServer")]
[assembly: InternalsVisibleTo("EntityFramework.Exceptions.PostgreSQL")]


namespace EntityFramework.Exceptions
{
    public abstract class ExceptionProcessorContextBase<T> : DbContext where T : Exception
    {
        private static readonly Dictionary<DatabaseError, Func<Exception, Exception>> ExceptionMapping = new Dictionary<DatabaseError, Func<Exception, Exception>>
        {
            {DatabaseError.MaxLength, exception => new MaxLengthException("", exception) },
            {DatabaseError.UniqueConstraint, exception => new UniqueConstraintException("Unique constraint violation", exception) },
            {DatabaseError.CannotInsertNull, exception => new CannotInsertNullException("Cannot insert null", exception) }
        };

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var baseException = ex.GetBaseException();

                var error = GetDatabaseError(baseException as T);

                if (error != null && ExceptionMapping.TryGetValue(error.Value, out var ctor))
                {
                    throw ctor(ex);
                }

                throw;
            }
        }

        internal abstract DatabaseError? GetDatabaseError(T exception);
    }

    internal enum DatabaseError
    {
        UniqueConstraint,
        CannotInsertNull,
        MaxLength
    }
}
