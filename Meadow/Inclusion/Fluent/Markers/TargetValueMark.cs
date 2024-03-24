using System;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Inclusion.Enums;

namespace Meadow.Inclusion.Fluent.Markers;

public record TargetValueMark(
    TargetTypes TargetType,
    Type? TargetModelType, 
    FieldKey? FieldKey,  
    Type? ValueType,
    string? Value);