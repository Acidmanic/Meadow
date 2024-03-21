using System;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Inclusion.Fluent.Markers;


internal record TargetValueMark(Type? TargetModelType, FieldKey? FieldKey, bool IsConstant, string? Value);