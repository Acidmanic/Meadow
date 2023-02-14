using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Contracts;

namespace Meadow.Scaffolding.Models
{
    /// <summary>
    /// This class would contain common information that might be needed for generating scripts. 
    /// </summary>
    public class ProcessedType
    {
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
        
        public List<Parameter> NoneIdParameters { get; set; }= new List<Parameter>();
        
        
        public List<Parameter> NoneIdUniqueParameters { get; set; }= new List<Parameter>();

        public NameConvention NameConvention { get; set; }
        
        public AccessNode IdField { get; set; }
        
        public bool HasId { get; set; }
        
        public Parameter IdParameter { get; set; }
        
    }
}