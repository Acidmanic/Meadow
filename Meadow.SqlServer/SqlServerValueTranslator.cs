using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Casting;
using Meadow.Configuration;
using Meadow.DataTypeMapping;

namespace Meadow.SqlServer;

public class SqlServerValueTranslator : ValueTranslatorBase
{
    public SqlServerValueTranslator(List<ICast> externalCasts) : base(externalCasts)
    {
    }
    
    public SqlServerValueTranslator(MeadowConfiguration configuration) : base(configuration)
    {
    }

    protected override string EscapedStringValueQuote => "''";

    public static SqlServerValueTranslator Create(MeadowConfiguration configuration)
        => new SqlServerValueTranslator(configuration.ExternalTypeCasts);
}