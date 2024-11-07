namespace EntityFramework.Exceptions.Common;

public enum DatabaseError
{
    UniqueConstraint,
    CannotInsertNull,
    MaxLength,
    NumericOverflow,
    ReferenceConstraint
}