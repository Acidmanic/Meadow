using Meadow.Attributes;

namespace Meadow.Configuration.ConfigurationRequests.Models
{
    public class MeadowDatabaseHistory
    {
        [UniqueField] public long Id { get; set; }

        public long ScriptOrder { get; set; }

        public string ScriptName { get; set; }

        public string Script { get; set; }
    }
}