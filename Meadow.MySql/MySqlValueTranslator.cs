using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Casting;
using Meadow.Configuration;
using Meadow.DataTypeMapping;

namespace Meadow.MySql;

public class MySqlValueTranslator : ValueTranslatorBase
{
    public MySqlValueTranslator(List<ICast> externalCasts) : base(externalCasts)
    {
    }

    protected override string EscapedStringValueQuote => "\\'";

    public static MySqlValueTranslator Create(MeadowConfiguration configuration)
        => new MySqlValueTranslator(configuration.ExternalTypeCasts);
}