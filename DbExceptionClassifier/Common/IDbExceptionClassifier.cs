using System.Data.Common;

namespace DbExceptionClassifier.Common
{
    public interface IDbExceptionClassifier
    {
        public bool IsReferenceConstraintError(DbException exception);
        public bool IsCannotInsertNullError(DbException exception);
        public bool IsNumericOverflowError(DbException exception);
        public bool IsUniqueConstraintError(DbException exception);
        public bool IsMaxLengthExceededError(DbException exception);
    }
}
