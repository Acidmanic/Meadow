using Meadow.Contracts;

namespace Meadow.Scaffolding.Models
{
    public class Parameter
    {

        public static readonly Parameter Null = new() {Name = string.Empty,Type = string.Empty,IdentifierStatus = ParameterIdentifierStatus.None,IsNumerical = false,StandardAddress = string.Empty};

        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public string StandardAddress { get; set; } = string.Empty;

        public bool IsNumerical { get; set; } 
        
        public ParameterIdentifierStatus IdentifierStatus { get; set; }

    }
}