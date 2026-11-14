using System.Data.Common;
using DbExceptionClassifier.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.Common;

sealed class ClassifierExceptionProcessorInterceptor(IDbExceptionClassifier exceptionClassifier)
    : ExceptionProcessorInterceptor<DbException>(exceptionClassifier);

/// <summary>
/// Extension methods for plugging a custom <see cref="IDbExceptionClassifier"/> into Entity Framework Core.
/// Use these when there is no dedicated EntityFrameworkCore.Exceptions package for your database provider
/// (for example, Devart dotConnect providers) or when you want to override how errors are classified.
/// </summary>
public static class CustomClassifierExceptionProcessorExtensions
{
    /// <summary>
    /// Translates database errors into typed exceptions using the supplied <paramref name="exceptionClassifier"/>.
    /// </summary>
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self, IDbExceptionClassifier exceptionClassifier)
        => self.AddInterceptors(new ClassifierExceptionProcessorInterceptor(exceptionClassifier));

    /// <summary>
    /// Translates database errors into typed exceptions using the supplied <paramref name="exceptionClassifier"/>.
    /// </summary>
    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self, IDbExceptionClassifier exceptionClassifier) where TContext : DbContext
        => self.AddInterceptors(new ClassifierExceptionProcessorInterceptor(exceptionClassifier));
}
