using System;
using Meadow.Inclusion.Enums;

namespace Meadow.Scaffolding.Translators;

public interface ISqlLanguageTranslator
{
    string ComparisonOperator(Operators opr, Type sourceType, Type targetType);
    
    string RelationString(BooleanRelation relation);
    
    string TableAliasQuot { get; }

    string QuotNames(string name);

}