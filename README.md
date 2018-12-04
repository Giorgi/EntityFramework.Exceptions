![EntityFramework.Exceptions](Icon.png "EntityFramework.Exceptions")

# EntityFramework.Exceptions
Handle database errors easily when working with Entity Framework. Supports SQLServer, PostgreSQL and MySql

### What does EntityFramework.Exceptions do?

When using Entity Framework for data access all database exceptions are wrapped in `DbUpdateException`. If you need to find 
whether the exception was caused by a unique constraint, value being too long or value missing for a required column you need to dig into 
the concrete `DbException` subclass instance and check the error code to determine the exact cause.

EntityFramework.Exceptions simplifies this by handling all the database specific details and throwing different exceptions. All you have
to do is inherit your `DbContext` from `ExceptionProcessorContext` and handle the exception(s) such as `UniqueConstraintException`,
`CannotInsertNullException`, `MaxLengthExceededException`, `NumericOverflowException` you need.
