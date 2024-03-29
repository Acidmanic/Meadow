
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.DataTypeMapping.Attributes;

namespace Meadow.Models
{
    public class MeadowDatabaseHistory
    {
        [UniqueMember]
        [AutoValuedMember]
        public long Id { get; set; }

        public int ScriptOrder { get; set; }

        public string ScriptName { get; set; }

        [IsLargeText]
        public string Script { get; set; }
    }
}