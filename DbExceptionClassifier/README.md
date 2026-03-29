![EntityFramework.Exceptions](https://raw.githubusercontent.com/Giorgi/EntityFramework.Exceptions/main/Icon.png "EntityFramework.Exceptions")

# DbExceptionClassifier

Classify ADO.NET database exceptions by error type. Works with any ADO.NET provider without requiring Entity Framework Core.

Supports **PostgreSQL**, **SQL Server**, **SQLite**, **Oracle**, **MySQL** (MySql.Data and MySqlConnector).

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square&logo=Apache)](../License.md)
[![Target](https://img.shields.io/static/v1?label=target&message=net10.0&color=512bd4&logo=.net&style=flat-square)](https://dotnet.microsoft.com/en-us/)

[![](https://img.shields.io/nuget/dt/DbExceptionClassifier.SqlServer.svg?label=DbExceptionClassifier.SqlServer&style=flat-square&logo=Microsoft-Sql-Server)](https://www.nuget.org/packages/DbExceptionClassifier.SqlServer/)
[![](https://img.shields.io/nuget/dt/DbExceptionClassifier.PostgreSQL.svg?label=DbExceptionClassifier.PostgreSQL&style=flat-square&logo=PostgreSQL)](https://www.nuget.org/packages/DbExceptionClassifier.PostgreSQL/)
[![](https://img.shields.io/nuget/dt/DbExceptionClassifier.MySQL.svg?label=DbExceptionClassifier.MySQL&style=flat-square&logo=MySQL&logoColor=white)](https://www.nuget.org/packages/DbExceptionClassifier.MySQL/)
[![](https://img.shields.io/nuget/dt/DbExceptionClassifier.MySQL.Pomelo.svg?label=DbExceptionClassifier.MySQL.Pomelo&style=flat-square&logo=MySQL&logoColor=white)](https://www.nuget.org/packages/DbExceptionClassifier.MySQL.Pomelo/)
[![](https://img.shields.io/nuget/dt/DbExceptionClassifier.Sqlite.svg?label=DbExceptionClassifier.Sqlite&style=flat-square&logo=Sqlite)](https://www.nuget.org/packages/DbExceptionClassifier.Sqlite/)
[![](https://img.shields.io/nuget/dt/DbExceptionClassifier.Oracle.svg?label=DbExceptionClassifier.Oracle&style=flat-square&logo=Oracle)](https://www.nuget.org/packages/DbExceptionClassifier.Oracle/)

## What does DbExceptionClassifier do?

When working with ADO.NET, database exceptions are provider-specific. To determine whether an error was caused by a unique constraint violation, a foreign key violation, or a null constraint, you need to inspect the provider-specific exception type and error codes.

DbExceptionClassifier provides a unified `IDbExceptionClassifier` interface that classifies `DbException` instances into common error types:

- **Unique constraint violation**
- **Reference (foreign key) constraint violation**
- **Cannot insert null**
- **Max length exceeded**
- **Numeric overflow**
- **Deadlock**

## Getting started

Install the package for your database:

```
dotnet add package DbExceptionClassifier.PostgreSQL
```

```
dotnet add package DbExceptionClassifier.SqlServer
```

```
dotnet add package DbExceptionClassifier.Sqlite
```

```
dotnet add package DbExceptionClassifier.Oracle
```

```
dotnet add package DbExceptionClassifier.MySQL
```

```
dotnet add package DbExceptionClassifier.MySQL.Pomelo
```

## Usage

Create an instance of the classifier for your database and use it to classify exceptions:

```csharp
var classifier = new PostgreSQLExceptionClassifier();

try
{
    // Execute your ADO.NET command
    await command.ExecuteNonQueryAsync();
}
catch (DbException ex) when (classifier.IsUniqueConstraintError(ex))
{
    // Handle unique constraint violation
}
catch (DbException ex) when (classifier.IsReferenceConstraintError(ex))
{
    // Handle foreign key violation
}
```

## IDbExceptionClassifier interface

```csharp
public interface IDbExceptionClassifier
{
    bool IsUniqueConstraintError(DbException exception);
    bool IsReferenceConstraintError(DbException exception);
    bool IsCannotInsertNullError(DbException exception);
    bool IsMaxLengthExceededError(DbException exception);
    bool IsNumericOverflowError(DbException exception);
    bool IsDeadlockError(DbException exception);
}
```

## CompositeExceptionClassifier

If your application connects to multiple database types, use `CompositeExceptionClassifier` to combine multiple classifiers into one. It delegates to each classifier and returns `true` if any of them matches:

```csharp
var classifier = new CompositeExceptionClassifier(
    new PostgreSQLExceptionClassifier(),
    new SqlServerExceptionClassifier()
);

try
{
    await command.ExecuteNonQueryAsync();
}
catch (DbException ex) when (classifier.IsUniqueConstraintError(ex))
{
    // Works regardless of which database threw the exception
}
```

> [!TIP]
> If you want to use another native SQLite binary instead of `e_sqlite3.dll` use the [DbExceptionClassifier.Sqlite.Core](https://www.nuget.org/packages/DbExceptionClassifier.Sqlite.Core) package. This package depends on `Microsoft.Data.Sqlite.Core`, which doesn't include the native SQLite binary so you can use any native binary you want.

## Entity Framework Core

If you are using Entity Framework Core, use the [EntityFrameworkCore.Exceptions](https://github.com/Giorgi/EntityFramework.Exceptions) packages instead. They build on top of DbExceptionClassifier and provide typed exceptions that inherit from `DbUpdateException`.
