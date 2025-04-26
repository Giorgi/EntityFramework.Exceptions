using DbExceptionClassifier.Oracle;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace EntityFramework.Exceptions.Oracle;

class OracleExceptionProcessorInterceptor() 
    : ExceptionProcessorInterceptor<OracleException>(new OracleExceptionClassifier());
    
public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self) 
        => self.AddInterceptors(new OracleExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext =>
        self.AddInterceptors(new OracleExceptionProcessorInterceptor());
}