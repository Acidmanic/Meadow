using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Meadow.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    
    public class ProductClassDal
    {
        [Key][UniqueField] public long Id { get; set; }

        public ICollection<ProductClassPropertyTag> DeclaredProperties { get; set; }
        
        public string Description { get; set; }
    }
}