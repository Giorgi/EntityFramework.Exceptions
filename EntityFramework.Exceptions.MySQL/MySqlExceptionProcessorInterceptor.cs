﻿using System;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

#if POMELO
using MySqlConnector;
namespace EntityFramework.Exceptions.MySQL.Pomelo;
#else
using MySql.Data.MySqlClient;
namespace EntityFramework.Exceptions.MySQL;
#endif

class MySqlExceptionProcessorInterceptor : ExceptionProcessorInterceptor<MySqlException>
{
    protected override DatabaseError? GetDatabaseError(MySqlException dbException)
    {

#if POMELO
            return dbException.ErrorCode switch
#else
        return (MySqlErrorCode)dbException.Number switch
#endif
        {
            MySqlErrorCode.ColumnCannotBeNull => DatabaseError.CannotInsertNull,
            MySqlErrorCode.DuplicateKeyEntry=> DatabaseError.UniqueConstraint,
            MySqlErrorCode.WarningDataOutOfRange => DatabaseError.NumericOverflow,
            MySqlErrorCode.DataTooLong => DatabaseError.MaxLength,
            MySqlErrorCode.NoReferencedRow => DatabaseError.ReferenceConstraint,
            MySqlErrorCode.RowIsReferenced => DatabaseError.ReferenceConstraint,
            MySqlErrorCode.NoReferencedRow2 => DatabaseError.ReferenceConstraint,
            MySqlErrorCode.RowIsReferenced2 => DatabaseError.ReferenceConstraint,
            _ => null
        };
    }
}

public static class ExceptionProcessorExtensions
{
    public static IServiceCollection AddExceptionProcessor(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.Add(ServiceDescriptor.Describe(typeof(IInterceptor), typeof(MySqlExceptionProcessorInterceptor), serviceLifetime));
        return services;
    }

    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self.AddInterceptors(new MySqlExceptionProcessorInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
    {
        return self.AddInterceptors(new MySqlExceptionProcessorInterceptor());
    }
}