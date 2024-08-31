using System.Collections.Generic;

namespace Meadow.Configuration;

public class MeadowConfigurationModel
{
    public string ConnectionString { get; set; } = string.Empty;

    public string BuildupScriptDirectory { get; set; } = "Scripts";

    public char DatabaseFieldNameDelimiter { get; set; } = '_';

    public MacroPolicies MacroPolicy { get; set; } = MacroPolicies.Ignore;

    public bool UsesLegacyConditionalStandardRelationalMapping { get; set; } = false;

    public Dictionary<string, int> ExternallyForcedColumnSizesByNodeAddress { get; set; } =
        new Dictionary<string, int>();
}