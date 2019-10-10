using System;
using System.Collections.Generic;
using System.Data.Common;
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

        private static readonly Dictionary<DatabaseError, Func<DbUpdateException, Exception>> ExceptionMapping = new Dictionary<DatabaseError, Func<DbUpdateException, Exception>>
        {
            {DatabaseError.MaxLength, exception => new MaxLengthExceededException("Maximum length exceeded", exception.InnerException) },
            {DatabaseError.UniqueConstraint, exception => new UniqueConstraintException("Unique constraint violation", exception.InnerException) },
            {DatabaseError.CannotInsertNull, exception => new CannotInsertNullException("Cannot insert null", exception.InnerException) },
            {DatabaseError.NumericOverflow, exception => new NumericOverflowException("Numeric overflow", exception.InnerException) },
            {DatabaseError.ReferenceConstraint, exception => new ReferenceConstraintException("Reference constraint violation", exception.InnerException) }
        };

        protected ExceptionProcessorStateManager(StateManagerDependencies dependencies) : base(dependencies)
        {
        }

        protected override int SaveChanges(IReadOnlyList<InternalEntityEntry> entriesToSave)
        {
            try
            {
                return base.SaveChanges(entriesToSave);
            }
            catch (DbUpdateException originalException)
            {
                var exception = GetConstructor(originalException);

                if (exception != null)
                {
                    throw exception;
                }

                throw;
            }
        }

        protected override async Task<int> SaveChangesAsync(IReadOnlyList<InternalEntityEntry> entriesToSave, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await base.SaveChangesAsync(entriesToSave, cancellationToken);
            }
            catch (DbUpdateException originalException)
            {
                var exception = GetConstructor(originalException);

                if (exception != null)
                {
                    throw exception;
                }

                throw;
            }
        }

        private Exception GetConstructor(DbUpdateException ex)
        {
            if (ex.GetBaseException() is T dbException && GetDatabaseError(dbException) is DatabaseError error && ExceptionMapping.TryGetValue(error, out var ctor))
            {
                return ctor(ex);
            }

            return null;
        }

        protected abstract DatabaseError? GetDatabaseError(T dbException);
    }
}
