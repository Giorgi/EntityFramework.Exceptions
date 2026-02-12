# EntityFramework.Exceptions

Typed exception handling for Entity Framework Core. Converts database-specific errors into strongly-typed .NET exceptions instead of generic `DbUpdateException`.

## Build & Test

```bash
dotnet restore                # Restore NuGet dependencies
dotnet build --no-restore     # Build all projects
dotnet test --no-build        # Run tests (requires Docker for Testcontainers)
```

Tests use **Testcontainers** and require a running Docker daemon. Each database provider (SQL Server, PostgreSQL, MySQL, Oracle, SQLite) spins up its own container.

## Project Structure

The solution (`EntityFramework.Exceptions.slnx`) has two main layers:

- **DbExceptionClassifier/** — Database-specific error code classification. Each provider implements `IDbExceptionClassifier` to map native error codes to a `DatabaseError` enum.
  - `Common/` — `IDbExceptionClassifier` interface
  - `SqlServer/`, `PostgreSQL/`, `MySQL/`, `MySQL.Pomelo/`, `Oracle/`, `Sqlite/` — Provider implementations

- **EntityFramework.Exceptions/** — EF Core integration via interceptors. Catches `DbException`, classifies it, and throws a typed exception.
  - `Common/` — Base `ExceptionProcessorInterceptor<T>`, exception classes (`UniqueConstraintException`, `CannotInsertNullException`, `MaxLengthExceededException`, `NumericOverflowException`, `ReferenceConstraintException`), `ExceptionFactory`
  - `SqlServer/`, `PostgreSQL/`, `MySQL/`, `MySQL.Pomelo/`, `Oracle/`, `Sqlite/` — Provider-specific interceptors and `UseExceptionProcessor()` extension methods
  - `Tests/` — xUnit test suite using Testcontainers

- **Directory.Build.props** — Shared build properties (target framework, version, NuGet metadata). All non-Common projects automatically reference their corresponding Common project.

## Architecture

1. **Interceptor pattern**: `ExceptionProcessorInterceptor<TProviderException>` implements `IDbCommandInterceptor` and `ISaveChangesInterceptor`
2. **Classification**: Each database provider has an `IDbExceptionClassifier` that maps native error codes to `DatabaseError` enum values
3. **Factory**: `ExceptionFactory` creates the appropriate typed exception
4. **Extension methods**: Each provider exposes `UseExceptionProcessor()` on `DbContextOptionsBuilder`

## Code Conventions

- **C# / .NET 8.0** with file-scoped namespaces, primary constructors, nullable reference types, and implicit usings
- **Naming**: `[Database]ExceptionClassifier`, `[Database]ExceptionProcessorInterceptor`, `ExceptionProcessorExtensions.UseExceptionProcessor()`
- **Test naming**: `[Scenario]Throws[ExceptionType]` (e.g., `UniqueColumnViolationThrowsUniqueConstraintException`)
- **MySQL.Pomelo** shares source files with **MySQL** via `<Link>` in .csproj and uses `#if POMELO` preprocessor directive
- Exception classes follow a standard pattern: inherit `DbUpdateException`, provide all standard constructor overloads, and optionally expose `ConstraintName`, `ConstraintProperties`, and `SchemaQualifiedTableName` properties

## Testing Notes

- Base test class `DatabaseTests` defines ~12 virtual test methods; provider-specific test classes inherit and override as needed
- **SQLite** does not populate `ConstraintName`/`ConstraintProperties` and does not enforce numeric overflow
- **SQL Server** skips numeric overflow tests (`ArgumentException` from SqlClient)
- **MySQL** primary key violations do not populate constraint properties
- Test fixtures use `IAsyncLifetime` for container lifecycle management
