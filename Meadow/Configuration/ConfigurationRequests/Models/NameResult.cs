using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Configuration.ConfigurationRequests.Models
{
    public class NameResult
    {
        [UniqueMember]
        public string Name { get; set; }
    }
}