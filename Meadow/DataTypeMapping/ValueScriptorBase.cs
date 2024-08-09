using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.Extensions;

namespace Meadow.DataTypeMapping;

public class ValueScriptorBase : IValueScriptor
{
    private readonly List<ICast> _externalCasts;


    public ValueScriptorBase(List<ICast> externalCasts)
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
        if (type == typeof(string)) return $"{StringQuote}{v}{StringQuote}";

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

    protected virtual char StringQuote => '"';
}