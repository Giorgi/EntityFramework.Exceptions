# AGENTS.md

This file provides guidance to AI coding agents when working with code in this repository.

## Build & Test Commands

```bash
# Build entire solution
dotnet build EntityFramework.Exceptions.slnx

# Run all tests (requires Docker for Testcontainers)
dotnet test EntityFramework.Exceptions/Tests/Tests.csproj

# Run a specific test class
dotnet test EntityFramework.Exceptions/Tests/Tests.csproj --filter ClassName=EntityFramework.Exceptions.Tests.PostgreSQLTests

# Run a specific test by name
dotnet test EntityFramework.Exceptions/Tests/Tests.csproj --filter Name=UniqueColumnViolationThrowsUniqueConstraintException

# Run tests with coverage (CI command)
dotnet test EntityFramework.Exceptions/Tests/Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:Exclude="[*]EntityFramework.Exceptions.Common.*Exception"
```

Tests use Testcontainers for PostgreSQL, SQL Server, MySQL, and Oracle (Docker required). SQLite tests use an in-memory shared database and don't need Docker.

## Architecture

The library catches EF Core `DbUpdateException` and rethrows typed exceptions (`UniqueConstraintException`, `ReferenceConstraintException`, `CannotInsertNullException`, `MaxLengthExceededException`, `NumericOverflowException`). All inherit from `DbUpdateException`.

### Two-layer design

**Layer 1 — `DbExceptionClassifier/`**: Pure database error classification with no EF Core dependency, shipped as separate packages (see `DbExceptionClassifier/README.md`). Each provider implements `IDbExceptionClassifier` (in `Common/`) to identify error types from the raw `DbException` (e.g., SQL error codes, PostgreSQL SqlState values). `CompositeExceptionClassifier` combines several classifiers (a match in any one wins).

The classifier interface also exposes `IsDeadlockError`, but the EF Core layer has no matching exception: `DeadlockException` was removed, so deadlocks are not translated by the interceptors. Adding a method to `IDbExceptionClassifier` means updating every provider classifier, `CompositeExceptionClassifier`, and the test stub in `Tests/CompositeExceptionClassifierTests.cs`.

**Layer 2 — `EntityFramework.Exceptions/`**: EF Core integration. A generic base interceptor `ExceptionProcessorInterceptor<TProviderException>` (in `Common/`) implements `ISaveChangesInterceptor` and `IDbCommandInterceptor`. It uses the classifier to detect error types, creates the typed exception via `ExceptionFactory`, then populates constraint metadata (`ConstraintName`, `ConstraintProperties`, `SchemaQualifiedTableName`) by matching the database exception message against the EF Core model's indexes and keys.

Each provider project is minimal — just a concrete interceptor subclass injecting its classifier, plus a `UseExceptionProcessor()` extension method on `DbContextOptionsBuilder`.

### Constraint property resolution

`SetConstraintDetails` in the base interceptor lazily builds a cached list of `IndexDetails` from:
- Unique indexes: `GetDeclaredIndexes().Where(index => index.IsUnique)` → `GetMappedTableIndexes()`
- Primary keys: `FindPrimaryKey()` → `GetName()`

It then matches the constraint name found in the provider exception message against this list. `ConstraintProperties` is only populated when a match is found. SQLite never populates these properties.

### MySQL/Pomelo code sharing

MySQL has two variants (MySql.Data and Pomelo). They share source files via `<Compile Include>` links with `#if POMELO` preprocessor directives. The Pomelo projects set `DefineConstants=POMELO`.

### Naming conventions

- NuGet packages: `EntityFrameworkCore.Exceptions.{Provider}` and `DbExceptionClassifier.{Provider}`
- Assembly names follow the same pattern (configured via `Directory.Build.props` using `$(MSBuildProjectName)`)
- Interceptors: `{Provider}ExceptionProcessorInterceptor`
- Classifiers: `{Provider}ExceptionClassifier`
- Test classes: `{Database}Tests` extending abstract `DatabaseTests`
- Test fixtures: `{Database}DemoContextFixture` implementing `IAsyncLifetime`

### Build configuration

Root `Directory.Build.props` sets the `net10.0` target, shared package metadata, and a `ProjectReference` to `..\Common\Common.csproj` for every non-Common project. Each layer folder has its own `Directory.Build.props` that configures assembly/package naming **and the version**: the two layers are versioned independently (`EntityFramework.Exceptions/` is `10.0.x`, tracking EF Core; `DbExceptionClassifier/` is `1.0.x`). Bump `Version`, `FileVersion`, `AssemblyVersion`, and `PackageReleaseNotes` together when releasing. Solution file is `EntityFramework.Exceptions.slnx` (modern XML format).
