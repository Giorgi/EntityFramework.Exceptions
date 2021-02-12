using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace EntityFramework.Exceptions.Common
{
    public abstract class ExceptionProcessorStateManager<T> : StateManager where T : DbException
    {
        protected internal enum DatabaseError
        {
            UniqueConstraint,
            CannotInsertNull,
            MaxLength,
            NumericOverflow,
            ReferenceConstraint
        }

        protected ExceptionProcessorStateManager(StateManagerDependencies dependencies) : base(dependencies)
        {

        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            try
            {
                return base.SaveChanges(acceptAllChangesOnSuccess);
            }
            catch (DbUpdateException originalException)
            {
                var exception = GetException(originalException);

                if (exception != null)
                {
                    throw exception;
                }

                throw;
            }
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (DbUpdateException originalException)
            {
                var exception = GetException(originalException);

                if (exception != null)
                {
                    throw exception;
                }

                throw;
            }
        }

        private Exception GetException(DbUpdateException ex)
        {
            if (ex.GetBaseException() is T dbException && GetDatabaseError(dbException) is DatabaseError error)
            {
                var entries = ex.Entries.Select(entry => base.GetOrCreateEntry(entry.Entity, entry.Metadata)).ToList();
                return ExceptionFactory.Create(error, ex, entries);
            }

            return null;
        }

        protected abstract DatabaseError? GetDatabaseError(T dbException);
    }
}
