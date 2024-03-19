using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFramework.Exceptions.Common;

public abstract class ExceptionProcessorInterceptor<T> : SaveChangesInterceptor where T : DbException
{
    private Dictionary<string, IReadOnlyList<IProperty>> uniqueIndexes;
    private Dictionary<string, IReadOnlyList<IProperty>> foreignKeys;

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

                switch (exception)
                {
                    case UniqueConstraintException uniqueConstraint when eventData.Context != null:
                        SetConstraintDetails(eventData.Context, uniqueConstraint, providerException);
                        break;
                    case ReferenceConstraintException referenceConstraint when eventData.Context != null:
                        SetConstraintDetails(eventData.Context, referenceConstraint, providerException);
                        break;
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

                switch (exception)
                {
                    case UniqueConstraintException uniqueConstraint when eventData.Context != null:
                        SetConstraintDetails(eventData.Context, uniqueConstraint, providerException);
                        break;
                    case ReferenceConstraintException referenceConstraint when eventData.Context != null:
                        SetConstraintDetails(eventData.Context, referenceConstraint, providerException);
                        break;
                }

                throw exception;
            }
        }

        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private void SetConstraintDetails(DbContext context, UniqueConstraintException exception, Exception providerException)
    {
        if (uniqueIndexes == null)
        {
            var indexes = context.Model.GetEntityTypes().SelectMany(x => x.GetIndexes().Where(index => index.IsUnique));
            uniqueIndexes = indexes.ToDictionary(x => x.GetDatabaseName()!, x => x.Properties);
        }

        var (key, value) = uniqueIndexes.FirstOrDefault(pair => providerException.Message.Contains(pair.Key));

        exception.ConstraintName = key;

        if (value != null)
        {
            exception.ConstraintProperties = value.Select(property => property.Name).ToList();
        }
    }

    private void SetConstraintDetails(DbContext context, ReferenceConstraintException exception, Exception providerException)
    {
        if (foreignKeys == null)
        {
            var keys = context.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys());
            foreignKeys = keys.ToDictionary(key => key.GetConstraintName(), key => key.Properties);
        }

        var (key, value) = foreignKeys.FirstOrDefault(pair => providerException.Message.Contains(pair.Key));

        exception.ConstraintName = key;

        if (value != null)
        {
            exception.ConstraintProperties = value.Select(property => property.Name).ToList();
        }
    }
}