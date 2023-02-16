using System;

namespace Meadow.DataTypeMapping.Attributes;


public class ForceDatabaseTypeAttribute:Attribute
{
    public ForceDatabaseTypeAttribute(string databaseTypeName)
    {
        DatabaseTypeName = databaseTypeName;
    }

    public string DatabaseTypeName { get; }
}