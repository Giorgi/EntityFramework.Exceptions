using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EntityFramework.Exceptions.Common
{
    public abstract class ExceptionProcessorInterceptor<T> : SaveChangesInterceptor where T : DbException
    {
        protected internal enum DatabaseError
        {
            UniqueConstraint,
            CannotInsertNull,
            MaxLength,
            NumericOverflow,
            ReferenceConstraint
        }

        protected abstract DatabaseError? GetDatabaseError(T dbException);

        /// <inheritdoc />
        public override void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            var dbUpdateException = eventData.Exception as DbUpdateException;
            var providerException = eventData.Exception.GetBaseException() as T;

            var error = GetDatabaseError(providerException);
            
            if (error != null && dbUpdateException != null)
            {
                var exception = ExceptionFactory.Create(error.Value, dbUpdateException, dbUpdateException.Entries);
                throw exception;
            }

            base.SaveChangesFailed(eventData);
        }

        /// <inheritdoc />
        public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = new CancellationToken())
        {
            var dbUpdateException = eventData.Exception as DbUpdateException;
            var providerException = eventData.Exception.GetBaseException() as T;

            var error = GetDatabaseError(providerException);

            if (error != null && dbUpdateException != null)
            {
                var exception = ExceptionFactory.Create(error.Value, dbUpdateException, dbUpdateException.Entries);
                throw exception;
            }

            return base.SaveChangesFailedAsync(eventData, cancellationToken);
        }
    }
}