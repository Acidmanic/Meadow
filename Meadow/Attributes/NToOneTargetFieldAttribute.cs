using System;

namespace Meadow.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class NToOneTargetFieldAttribute : Attribute
{
    
    public string FieldName { get; }
    
    public NToOneTargetFieldAttribute(string fieldName)
    {
        FieldName = fieldName;
    }
}