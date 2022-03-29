﻿using EntityFramework.Exceptions.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using static SQLitePCL.raw;

namespace EntityFramework.Exceptions.Sqlite;

class SqliteExceptionProcessorInterceptor : ExceptionProcessorInterceptor<SqliteException>
{
    protected override DatabaseError? GetDatabaseError(SqliteException dbException)
    {
        if (dbException.SqliteErrorCode == SQLITE_CONSTRAINT || dbException.SqliteErrorCode == SQLITE_TOOBIG)
        {
            return dbException.SqliteExtendedErrorCode switch
            {
                SQLITE_TOOBIG => DatabaseError.MaxLength,
                SQLITE_CONSTRAINT_NOTNULL => DatabaseError.CannotInsertNull,
                SQLITE_CONSTRAINT_UNIQUE => DatabaseError.UniqueConstraint,
                SQLITE_CONSTRAINT_PRIMARYKEY => DatabaseError.UniqueConstraint,
                SQLITE_CONSTRAINT_FOREIGNKEY => DatabaseError.ReferenceConstraint,
                _ => null
            };
        }

        return null;
    }
}

public static class ExceptionProcessorExtensions
{
    public static IServiceCollection AddExceptionProcessor(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.Add(ServiceDescriptor.Describe(typeof(IInterceptor), typeof(SqliteExceptionProcessorInterceptor), serviceLifetime));
        return services;
    }

    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self.AddInterceptors(new SqliteExceptionProcessorInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
    {
        return self.AddInterceptors(new SqliteExceptionProcessorInterceptor());
    }
}