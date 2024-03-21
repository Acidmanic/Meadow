using System;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Inclusion.Fluent.Markers;

namespace Meadow.Inclusion.Fluent;

internal class InclusionCondition
{
    public FieldKey SourceField { get; set; }
    
    public Type SourceModelType { get; set; }
    
    public Operators Operator { get; set; }
    
    public TargetValueMark TargetValue { get; set; }
}