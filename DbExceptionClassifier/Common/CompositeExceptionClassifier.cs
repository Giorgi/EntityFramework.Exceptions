using System.Data.Common;

namespace DbExceptionClassifier.Common;

public class CompositeExceptionClassifier(params IDbExceptionClassifier[] classifiers) : IDbExceptionClassifier
{
    public bool IsReferenceConstraintError(DbException exception) => classifiers.Any(c => c.IsReferenceConstraintError(exception));
    public bool IsCannotInsertNullError(DbException exception) => classifiers.Any(c => c.IsCannotInsertNullError(exception));
    public bool IsNumericOverflowError(DbException exception) => classifiers.Any(c => c.IsNumericOverflowError(exception));
    public bool IsUniqueConstraintError(DbException exception) => classifiers.Any(c => c.IsUniqueConstraintError(exception));
    public bool IsMaxLengthExceededError(DbException exception) => classifiers.Any(c => c.IsMaxLengthExceededError(exception));
    public bool IsDeadlockError(DbException exception) => classifiers.Any(c => c.IsDeadlockError(exception));
}
