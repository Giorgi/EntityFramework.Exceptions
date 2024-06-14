using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFramework.Exceptions.Common;

internal class IndexDetails(string name, string schemaQualifiedTableName, IReadOnlyList<IProperty> properties)
{
    public string Name { get; init; } = name;
    public string SchemaQualifiedTableName { get; init; } = schemaQualifiedTableName;
    public IReadOnlyList<IProperty> Properties { get; } = properties;

    protected bool Equals(IndexDetails other)
    {
        return Name == other.Name && SchemaQualifiedTableName == other.SchemaQualifiedTableName;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((IndexDetails)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, SchemaQualifiedTableName);
    }
}