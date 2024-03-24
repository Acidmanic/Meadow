using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Inclusion.Enums;

namespace Meadow.Inclusion.Fluent;

public class InclusionRecord
{
    public FieldKey IncludedField { get; set; }
    
    public List<InclusionCondition> Conditions { get; set; }
    
    public Type Type { get; set; }
    
}