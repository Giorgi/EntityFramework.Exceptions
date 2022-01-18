﻿using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.Exceptions.SqlServer
{
    class SqlServerExceptionProcessorStateManager: ExceptionProcessorStateManager<SqlException>
    {
        public SqlServerExceptionProcessorStateManager(StateManagerDependencies dependencies) : base(dependencies)
        {
        }

        private const int ReferenceConstraint = 547;
        private const int CannotInsertNull = 515;
        private const int CannotInsertDuplicateKeyUniqueIndex = 2601;
        private const int CannotInsertDuplicateKeyUniqueConstraint = 2627;
        private const int ArithmeticOverflow = 8115;
        private const int StringOrBinaryDataWouldBeTruncated = 8152;

        protected override DatabaseError? GetDatabaseError(SqlException dbException)
        {
            switch (dbException.Number)
            {
                case ReferenceConstraint:
                    return DatabaseError.ReferenceConstraint;
                case CannotInsertNull:
                    return DatabaseError.CannotInsertNull;
                case CannotInsertDuplicateKeyUniqueIndex:
                case CannotInsertDuplicateKeyUniqueConstraint:
                    return DatabaseError.UniqueConstraint;
                case ArithmeticOverflow:
                    return DatabaseError.NumericOverflow;
                case StringOrBinaryDataWouldBeTruncated:
                    return DatabaseError.MaxLength;
                default:
                    return null;
            }
        }
    }

    public static class ExceptionProcessorExtensions
    {
        public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        {
            self.ReplaceService<IStateManager, SqlServerExceptionProcessorStateManager>();
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            self.ReplaceService<IStateManager, SqlServerExceptionProcessorStateManager>();
            return self;
        }

        /// <summary>
        /// Adds the exception processor. Needed only in advanced scenarios when EF is configured with UseInternalServiceProvider(IServiceProvider).
        /// </summary>
        public static IServiceCollection AddExceptionProcessor(this IServiceCollection self)
        {
            self.AddScoped<IStateManager, SqlServerExceptionProcessorStateManager>();
            return self;
        }
    }
}
