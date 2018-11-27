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

                if (error != null)
                {

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
