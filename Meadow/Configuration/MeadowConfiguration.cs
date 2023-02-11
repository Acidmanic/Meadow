namespace Meadow.Configuration
{
    public class MeadowConfiguration
    {
        public string ConnectionString { get; set; }
        
        public string BuildupScriptDirectory { get; set; }

        public char DatabaseFieldNameDelimiter { get; set; } = '_';

        public MacroPolicies MacroPolicy { get; set; } = MacroPolicies.Ignore;
    }
}