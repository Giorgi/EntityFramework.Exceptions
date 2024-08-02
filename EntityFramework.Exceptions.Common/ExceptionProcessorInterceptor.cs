using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFramework.Exceptions.Common;

public abstract class ExceptionProcessorInterceptor<T> : SaveChangesInterceptor where T : DbException
{
    private List<IndexDetails> uniqueIndexDetailsList;
    private List<ForeignKeyDetails> foreignKeyDetailsList;

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
        if (uniqueIndexDetailsList == null)
        {
            var indexes = context.Model.GetEntityTypes().SelectMany(x => x.GetDeclaredIndexes().Where(index => index.IsUnique));

            var mappedIndexes = indexes.SelectMany(index => index.GetMappedTableIndexes(), (index, tableIndex) => new { tableIndex, index.Properties });

            uniqueIndexDetailsList = mappedIndexes.Select(arg => new IndexDetails(arg.tableIndex.Name, arg.tableIndex.Table.SchemaQualifiedName, arg.Properties)).ToList();
        }

        var matchingIndexes = uniqueIndexDetailsList.Where(index => providerException.Message.Contains(index.Name, StringComparison.OrdinalIgnoreCase)).ToList();
        var match = matchingIndexes.Count == 1 ? matchingIndexes[0] : matchingIndexes.FirstOrDefault(index => providerException.Message.Contains(index.SchemaQualifiedTableName, StringComparison.OrdinalIgnoreCase));

        if (match != null)
        {
            exception.ConstraintName = match.Name;
            exception.ConstraintProperties = match.Properties.Select(property => property.Name).ToList();
            exception.SchemaQualifiedTableName = match.SchemaQualifiedTableName;
        }
    }

    private void SetConstraintDetails(DbContext context, ReferenceConstraintException exception, Exception providerException)
    {
        if (foreignKeyDetailsList == null)
        {
            var keys = context.Model.GetEntityTypes().SelectMany(x => x.GetDeclaredForeignKeys());

            var mappedConstraints = keys.SelectMany(index => index.GetMappedConstraints(), (index, constraint) => new { constraint, index.Properties });

            foreignKeyDetailsList = mappedConstraints.Select(arg => new ForeignKeyDetails(arg.constraint.Name, arg.constraint.Table.SchemaQualifiedName, arg.Properties)).ToList();
        }

        var matchingForeignKeys = foreignKeyDetailsList.Where(foreignKey => providerException.Message.Contains(foreignKey.Name, StringComparison.OrdinalIgnoreCase)).ToList();
        var match = matchingForeignKeys.Count == 1 ? matchingForeignKeys[0] : matchingForeignKeys.FirstOrDefault(foreignKey => providerException.Message.Contains(foreignKey.SchemaQualifiedTableName, StringComparison.OrdinalIgnoreCase));

        if (match != null)
        {
            exception.ConstraintName = match.Name;
            exception.ConstraintProperties = match.Properties.Select(property => property.Name).ToList();
            exception.SchemaQualifiedTableName = match.SchemaQualifiedTableName;
        }
    }
}