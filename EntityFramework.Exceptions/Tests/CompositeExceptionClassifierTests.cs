using System.Data.Common;
using DbExceptionClassifier.Common;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

public class CompositeExceptionClassifierTests
{
    [Fact]
    public void ReturnsTrueWhenAnyClassifierMatches()
    {
        var classifier = new CompositeExceptionClassifier(
            new StubClassifier(),
            new StubClassifier(uniqueConstraint: true)
        );

        Assert.True(classifier.IsUniqueConstraintError(new StubDbException()));
    }

    [Fact]
    public void ReturnsFalseWhenNoClassifierMatches()
    {
        var classifier = new CompositeExceptionClassifier(
            new StubClassifier(),
            new StubClassifier()
        );

        Assert.False(classifier.IsUniqueConstraintError(new StubDbException()));
    }

    [Fact]
    public void ReturnsFalseWhenEmpty()
    {
        var classifier = new CompositeExceptionClassifier();

        Assert.False(classifier.IsUniqueConstraintError(new StubDbException()));
        Assert.False(classifier.IsReferenceConstraintError(new StubDbException()));
        Assert.False(classifier.IsCannotInsertNullError(new StubDbException()));
        Assert.False(classifier.IsMaxLengthExceededError(new StubDbException()));
        Assert.False(classifier.IsNumericOverflowError(new StubDbException()));
        Assert.False(classifier.IsDeadlockError(new StubDbException()));
    }

    [Fact]
    public void DelegatesAllMethodsCorrectly()
    {
        var classifier = new CompositeExceptionClassifier(new StubClassifier(
            uniqueConstraint: true,
            referenceConstraint: true,
            cannotInsertNull: true,
            maxLengthExceeded: true,
            numericOverflow: true,
            deadlock: true
        ));

        var exception = new StubDbException();

        Assert.True(classifier.IsUniqueConstraintError(exception));
        Assert.True(classifier.IsReferenceConstraintError(exception));
        Assert.True(classifier.IsCannotInsertNullError(exception));
        Assert.True(classifier.IsMaxLengthExceededError(exception));
        Assert.True(classifier.IsNumericOverflowError(exception));
        Assert.True(classifier.IsDeadlockError(exception));
    }

    [Fact]
    public void MatchesCorrectErrorTypeAcrossClassifiers()
    {
        var classifier = new CompositeExceptionClassifier(
            new StubClassifier(uniqueConstraint: true),
            new StubClassifier(referenceConstraint: true)
        );

        var exception = new StubDbException();

        Assert.True(classifier.IsUniqueConstraintError(exception));
        Assert.True(classifier.IsReferenceConstraintError(exception));
        Assert.False(classifier.IsDeadlockError(exception));
    }

    private class StubDbException : DbException;

    private class StubClassifier(
        bool uniqueConstraint = false,
        bool referenceConstraint = false,
        bool cannotInsertNull = false,
        bool maxLengthExceeded = false,
        bool numericOverflow = false,
        bool deadlock = false) : IDbExceptionClassifier
    {
        public bool IsUniqueConstraintError(DbException exception) => uniqueConstraint;
        public bool IsReferenceConstraintError(DbException exception) => referenceConstraint;
        public bool IsCannotInsertNullError(DbException exception) => cannotInsertNull;
        public bool IsMaxLengthExceededError(DbException exception) => maxLengthExceeded;
        public bool IsNumericOverflowError(DbException exception) => numericOverflow;
        public bool IsDeadlockError(DbException exception) => deadlock;
    }
}
