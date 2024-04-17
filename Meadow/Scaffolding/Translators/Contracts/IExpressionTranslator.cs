using System;
using Meadow.Inclusion.Enums;

namespace Meadow.Scaffolding.Translators.Contracts;

public interface IExpressionTranslator
{
    string EscapedSingleQuote { get; }

    string EmptyConditionExpression => "";

    string EmptyOrderExpression(Type entityType);
    
    string ComparisonOperator(Operators opr, Type sourceType, Type targetType);
    
    string RelationString(BooleanRelation relation);
}