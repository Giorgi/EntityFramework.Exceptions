﻿using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.Exceptions.SqlServer;

class SqlServerExceptionProcessorInterceptor: ExceptionProcessorInterceptor<SqlException>
{
    private const int ReferenceConstraint = 547;
    private const int CannotInsertNull = 515;
    private const int CannotInsertDuplicateKeyUniqueIndex = 2601;
    private const int CannotInsertDuplicateKeyUniqueConstraint = 2627;
    private const int ArithmeticOverflow = 8115;
    private const int StringOrBinaryDataWouldBeTruncated = 8152;

    protected override DatabaseError? GetDatabaseError(SqlException dbException)
    {
        return dbException.Number switch
        {
            ReferenceConstraint => DatabaseError.ReferenceConstraint,
            CannotInsertNull => DatabaseError.CannotInsertNull,
            CannotInsertDuplicateKeyUniqueIndex => DatabaseError.UniqueConstraint,
            CannotInsertDuplicateKeyUniqueConstraint => DatabaseError.UniqueConstraint,
            ArithmeticOverflow => DatabaseError.NumericOverflow,
            StringOrBinaryDataWouldBeTruncated => DatabaseError.MaxLength,
            _ => null
        };
    }
}

public static class ExceptionProcessorExtensions
{
    public static IServiceCollection AddExceptionProcessor(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.Add(ServiceDescriptor.Describe(typeof(IInterceptor), typeof(SqlServerExceptionProcessorInterceptor), serviceLifetime));
        return services;
    }

    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
    {
        return self.AddInterceptors(new SqlServerExceptionProcessorInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
    {
        return self.AddInterceptors(new SqlServerExceptionProcessorInterceptor());
    }
}