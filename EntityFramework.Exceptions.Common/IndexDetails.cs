using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFramework.Exceptions.Common;

internal class IndexDetails
{
    public IndexDetails(string name, string schemaQualifiedTableName, IReadOnlyList<IProperty> properties)
    {
        Name = name;
        SchemaQualifiedTableName = schemaQualifiedTableName;
        Properties = properties;
    }

    public string Name { get; init; }
    public string SchemaQualifiedTableName { get; init; }
    public IReadOnlyList<IProperty> Properties { get; }
}