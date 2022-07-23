
using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Models
{
    public class MeadowDatabaseHistory
    {
        [UniqueMember]
        [AutoValuedMember]
        public long Id { get; set; }

        public int ScriptOrder { get; set; }

        public string ScriptName { get; set; }

        public string Script { get; set; }
    }
}