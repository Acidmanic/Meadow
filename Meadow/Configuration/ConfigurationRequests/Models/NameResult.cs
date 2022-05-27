using Meadow.Attributes;

namespace Meadow.Configuration.ConfigurationRequests.Models
{
    public class NameResult
    {
        [UniqueField]
        public string Name { get; set; }
    }
}