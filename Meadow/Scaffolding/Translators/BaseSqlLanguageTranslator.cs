using System;
using Meadow.Inclusion.Enums;

namespace Meadow.Scaffolding.Translators;

//WIP: Not used
public abstract class BaseSqlLanguageTranslator:ISqlLanguageTranslator
{
    public virtual string ComparisonOperator(Operators opr, Type sourceType, Type targetType)
    {
        var stringType = typeof(string);

        var isString = sourceType == stringType || targetType == stringType;

        var equality = isString ? "like" : "=";

        var inEquality = isString ? "NOT LIKE" : "!=";

        if (opr == Operators.IsEqualTo)
        {
            return equality;
        }
        else if (opr == Operators.IsNotEqualTo)
        {
            return inEquality;
        }
        else if (opr == Operators.IsGreaterThan)
        {
            return ">";
        }
        else if (opr == Operators.IsSmallerThan)
        {
            return "<";
        }
        else if (opr == Operators.IsGreaterOrEqualTo)
        {
            return ">=";
        }
        else if (opr == Operators.IsSmallerOrEqualTo)
        {
            return "<=";
        }

        return "=";
    }

    public virtual string RelationString(BooleanRelation relation)
    {
        if (relation == BooleanRelation.And) return "AND";

        if (relation == BooleanRelation.Or) return "OR";

        return "";
    }
    
    public virtual string TableAliasQuot { get; } = "'";
    
    public virtual string QuotNames(string name)
    {
        return name;
    }
}