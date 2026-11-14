using System.Data.Common;
using DbExceptionClassifier.Common;
using DotNet.Testcontainers.Containers;
using EntityFramework.Exceptions.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static SQLitePCL.raw;

namespace EntityFramework.Exceptions.Tests;

// Runs the SQLite suite through the generic UseExceptionProcessor(IDbExceptionClassifier) overload
// with a classifier defined outside the library, the same way a user would plug in an unsupported provider.
public class CustomClassifierTests(CustomClassifierDemoContextFixture fixture)
    : SqliteTestsBase(fixture), IClassFixture<CustomClassifierDemoContextFixture>;

public class CustomClassifierDemoContextFixture : DemoContextFixture<IContainer>
{
    private const string ConnectionString = "DataSource=file:customclassifier?mode=memory&cache=shared";

    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(DbContextOptionsBuilder<DemoContext> builder, string connectionString)
        => builder.UseSqlite(ConnectionString).UseExceptionProcessor(new UserDefinedSqliteClassifier());

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(DbContextOptionsBuilder builder, string connectionString)
        => builder.UseSqlite(ConnectionString).UseExceptionProcessor(new UserDefinedSqliteClassifier());
}

class UserDefinedSqliteClassifier : IDbExceptionClassifier
{
    public bool IsReferenceConstraintError(DbException exception) => exception is SqliteException { SqliteExtendedErrorCode: SQLITE_CONSTRAINT_FOREIGNKEY };
    public bool IsCannotInsertNullError(DbException exception) => exception is SqliteException { SqliteExtendedErrorCode: SQLITE_CONSTRAINT_NOTNULL };
    public bool IsNumericOverflowError(DbException exception) => false;
    public bool IsUniqueConstraintError(DbException exception) => exception is SqliteException { SqliteExtendedErrorCode: SQLITE_CONSTRAINT_UNIQUE or SQLITE_CONSTRAINT_PRIMARYKEY };
    public bool IsMaxLengthExceededError(DbException exception) => exception is SqliteException { SqliteExtendedErrorCode: SQLITE_TOOBIG };
    public bool IsDeadlockError(DbException exception) => false;
}
