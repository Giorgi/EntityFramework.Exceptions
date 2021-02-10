using System;
using System.Collections.Generic;
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
        protected enum DatabaseError
        {
            UniqueConstraint,
            CannotInsertNull,
            MaxLength,
            NumericOverflow,
            ReferenceConstraint
        }

        private static readonly Dictionary<DatabaseError, Func<DbUpdateException, List<InternalEntityEntry>, Exception>> ExceptionMapping = new Dictionary<DatabaseError, Func<DbUpdateException, List<InternalEntityEntry>, Exception>>
        {
            {DatabaseError.MaxLength, (exception, entries) => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException, entries) },
            {DatabaseError.UniqueConstraint, (exception, entries) => new UniqueConstraintException("Unique constraint violation", exception.InnerException, entries) },
            {DatabaseError.CannotInsertNull, (exception, entries) => new CannotInsertNullException("Cannot insert null", exception.InnerException, entries) },
            {DatabaseError.NumericOverflow, (exception, entries) => new NumericOverflowException("Numeric overflow", exception.InnerException, entries) },
            {DatabaseError.ReferenceConstraint, (exception, entries) => new ReferenceConstraintException("Reference constraint violation", exception.InnerException, entries) }
        };

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
            if (ex.GetBaseException() is T dbException && GetDatabaseError(dbException) is DatabaseError error && ExceptionMapping.TryGetValue(error, out var ctor))
            {
                var entries = ex.Entries.Select(entry => base.GetOrCreateEntry(entry.Entity, entry.Metadata)).ToList();
                return ctor(ex, entries);
            }

            return null;
        }

        protected abstract DatabaseError? GetDatabaseError(T dbException);
    }
}
