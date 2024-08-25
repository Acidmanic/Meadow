using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Casting;
using Meadow.Configuration;
using Meadow.DataTypeMapping;

namespace Meadow.SQLite;

public class SqLiteValueTranslator : ValueTranslatorBase
{
    public SqLiteValueTranslator(List<ICast> externalCasts) : base(externalCasts)
    {
    }
    
    public SqLiteValueTranslator(MeadowConfiguration configuration) : base(configuration)
    {
    }

    protected override string EscapedStringValueQuote => "\\'";

    public static SqLiteValueTranslator Create(MeadowConfiguration configuration)
        => new SqLiteValueTranslator(configuration.ExternalTypeCasts);
}