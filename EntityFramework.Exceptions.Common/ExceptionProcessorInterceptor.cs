using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EntityFramework.Exceptions.Common;

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

        if (eventData.Exception.GetBaseException() is T providerException)
        {
            var error = GetDatabaseError(providerException);

            if (error != null && dbUpdateException != null)
            {
                var exception = ExceptionFactory.Create(error.Value, dbUpdateException, dbUpdateException.Entries);

                if (exception is UniqueConstraintException uniqueConstraint && eventData.Context != null)
                {
                    var indexes = eventData.Context.Model.GetEntityTypes().SelectMany(x => x.GetIndexes().Where(index => index.IsUnique));
                    var indexNames = indexes.ToDictionary(x => x.GetDatabaseName()!, x => x.Properties);

                    var (key, value) = indexNames.FirstOrDefault(pair => providerException.Message.Contains(pair.Key));

                    uniqueConstraint.ConstraintName = key;
                }

                throw exception;
            }
        }

        base.SaveChangesFailed(eventData);
    }

    /// <inheritdoc />
    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = new CancellationToken())
    {
        var dbUpdateException = eventData.Exception as DbUpdateException;

        if (eventData.Exception.GetBaseException() is T providerException)
        {
            var error = GetDatabaseError(providerException);

            if (error != null && dbUpdateException != null)
            {
                var exception = ExceptionFactory.Create(error.Value, dbUpdateException, dbUpdateException.Entries);
                throw exception;
            }
        }

        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }
}