using Meadow.Contracts;

namespace Meadow.Scaffolding.Models
{
    public class Parameter
    {
        
        public string Name { get; set; }
        public string Type { get; set; }
        
        public string StandardAddress { get; set; }
        
        public bool IsNumerical { get; set; }
        
        public ParameterIdentifierStatus IdentifierStatus { get; set; }

    }
}