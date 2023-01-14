using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.Bug2
{
    public class Project
    {
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [AutoValuedMember]
        [UniqueMember]
        public long Id { get; set; }
        
        public List<Goal> Goals { get; set; }
        
        public List<Iteration> Iterations { get; set; }
    }
}