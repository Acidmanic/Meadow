using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    
    public class ProductClassDal
    {
        [Key][UniqueMember][AutoValuedMember] public long Id { get; set; }

        public ICollection<ProductClassPropertyTag> DeclaredProperties { get; set; }
        
        public string Description { get; set; }
        
        public QuantifierDal Quantifier { get; set; }
        
        public long QuantifierId { get; set; }
    }
}