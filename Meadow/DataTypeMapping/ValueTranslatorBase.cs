using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.Extensions;

namespace Meadow.DataTypeMapping;

public abstract class ValueTranslatorBase : IValueTranslator
{
    private readonly List<ICast> _externalCasts;


    public ValueTranslatorBase(List<ICast> externalCasts)
    {
        _externalCasts = externalCasts;
    }

    public string Translate(object? value)
    {
        if (value is { } v)
        {
            var translatingType = v.GetType();

            var translatingObject = v;

            var altered = translatingType.GetAlteredOrOriginal();

            if (altered is { } alteredType && alteredType != translatingType)
            {
                translatingObject = v.CastTo(alteredType, _externalCasts);

                translatingType = alteredType;
            }

            return Translate(translatingType, translatingObject);
        }

        return TranslateNull();
    }


    private string Translate(Type type, object v)
    {
        if (type == typeof(string))
        {
            var stringValue = (v as string)!;
            
            var escaped = stringValue.Replace($"{StringQuote}", EscapedStringValueQuote);

            return $"{StringQuote}{escaped}{StringQuote}";
        }

        if (type == typeof(bool)) return TranslateBoolean((bool)v);

        if (TypeCheck.IsNumerical(type)) return $"{v}";

        return $"{v}";
    }


    protected virtual string TranslateBoolean(bool value)
    {
        if (value)
        {
            return "1";
        }

        return "0";
    }

    protected virtual string TranslateNull() => "null";

    protected virtual char StringQuote => '\'';
    
    protected abstract string EscapedStringValueQuote { get; }
}