using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Inclusion.Fluent;

internal class InclusionRecord
{
    public FieldKey IncludedField { get; set; }
    
    public List<InclusionCondition> Conditions { get; set; }
    
    public Type Type { get; set; }
}