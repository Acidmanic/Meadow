using System;

namespace Meadow.Attributes;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public class OneToMany : Attribute
{
    
    public string ReferenceFieldName { get; }
    
    public OneToMany(string referenceFieldName)
    {
        ReferenceFieldName = referenceFieldName;
    }
}