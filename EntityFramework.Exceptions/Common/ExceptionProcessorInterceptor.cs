using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DbExceptionClassifier.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EntityFramework.Exceptions.Common;

public abstract class ExceptionProcessorInterceptor<TProviderException>(IDbExceptionClassifier exceptionClassifier)
    : IDbCommandInterceptor, ISaveChangesInterceptor where TProviderException : DbException
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

    /// <inheritdoc />
    public void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        ProcessException(eventData.Exception, eventData.Context);
    }

    /// <inheritdoc />
    public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = new CancellationToken())
    {
        ProcessException(eventData.Exception, eventData.Context);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        ProcessException(eventData.Exception, eventData.Context);
    }

    /// <inheritdoc />
    public Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = new CancellationToken())
    {
        ProcessException(eventData.Exception, eventData.Context);
        return Task.CompletedTask;
    }

    protected DatabaseError? GetDatabaseError(TProviderException dbException)
    {
        if (exceptionClassifier.IsMaxLengthExceededError(dbException)) return DatabaseError.MaxLength;
        if (exceptionClassifier.IsNumericOverflowError(dbException)) return DatabaseError.NumericOverflow;
        if (exceptionClassifier.IsCannotInsertNullError(dbException)) return DatabaseError.CannotInsertNull;
        if (exceptionClassifier.IsUniqueConstraintError(dbException)) return DatabaseError.UniqueConstraint;
        if (exceptionClassifier.IsReferenceConstraintError(dbException)) return DatabaseError.ReferenceConstraint;

        return null;
    }

    [StackTraceHidden]
    private void ProcessException(Exception eventException, DbContext eventContext)
    {
        if (eventException?.GetBaseException() is not TProviderException providerException) return;

        var error = GetDatabaseError(providerException);

        if (error == null) return;

        var updateException = eventException as DbUpdateException;
        var exception = ExceptionFactory.Create(error.Value, eventException, updateException?.Entries);

        switch (exception)
        {
            case UniqueConstraintException uniqueConstraint when eventContext != null:
                SetConstraintDetails(eventContext, uniqueConstraint, providerException);
                break;
            case ReferenceConstraintException referenceConstraint when eventContext != null:
                SetConstraintDetails(eventContext, referenceConstraint, providerException);
                break;
        }

        throw exception;
    }

    private void SetConstraintDetails(DbContext context, UniqueConstraintException exception, TProviderException providerException)
    {
        if (uniqueIndexDetailsList == null)
        {
            var indexes = context.Model.GetEntityTypes().SelectMany(x => x.GetDeclaredIndexes().Where(index => index.IsUnique));

            var mappedIndexes = indexes.SelectMany(index => index.GetMappedTableIndexes(), 
                (index, tableIndex) => new IndexDetails(tableIndex.Name, tableIndex.Table.SchemaQualifiedName, index.Properties));
            
            var primaryKeys = context.Model.GetEntityTypes().SelectMany(x =>
            {
                var primaryKey = x.FindPrimaryKey();
                if (primaryKey is null)
                {
                    return Array.Empty<IndexDetails>();
                }

                var primaryKeyName = primaryKey.GetName();

                if (primaryKeyName is null)
                {
                    return Array.Empty<IndexDetails>();
                }

                return [new IndexDetails(primaryKeyName, x.GetSchemaQualifiedTableName(), primaryKey.Properties)];
            });

            uniqueIndexDetailsList = mappedIndexes
                .Union(primaryKeys)
                .ToList();
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

    private void SetConstraintDetails(DbContext context, ReferenceConstraintException exception, TProviderException providerException)
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