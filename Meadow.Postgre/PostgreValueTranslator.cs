using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Casting;
using Meadow.Configuration;
using Meadow.DataTypeMapping;

namespace Meadow.Postgre;

public class PostgreValueTranslator : ValueTranslatorBase
{
    public PostgreValueTranslator(List<ICast> externalCasts) : base(externalCasts)
    {
    }

    protected override string EscapedStringValueQuote => "\\'";

    public static PostgreValueTranslator Create(MeadowConfiguration configuration)
        => new PostgreValueTranslator(configuration.ExternalTypeCasts);

    protected override string TranslateBoolean(bool value)
        => value ? "true" : "false";
}