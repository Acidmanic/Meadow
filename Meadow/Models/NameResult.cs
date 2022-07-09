using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Models
{
    public class NameResult
    {
        [UniqueMember]
        public string Name { get; set; }
    }
}