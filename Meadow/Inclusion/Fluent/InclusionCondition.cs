using System;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Inclusion.Fluent.Markers;

namespace Meadow.Inclusion.Fluent;

internal class InclusionCondition
{
    public FieldKey SourceField { get; set; }
    
    public Type SourceModelType { get; set; }
    
    public Operators Operator { get; set; }
    
    public TargetValueMark Target { get; set; }


    public Type GetSourceType() => new ObjectEvaluator(SourceModelType).Map.NodeByKey(SourceField).Type;

    public Type GetTargetType()
    {
        if (Target.IsConstant)
        {
            return Target.ValueType!;
        }
        else
        {
            return Target.TargetModelType!;
        }
    }
}