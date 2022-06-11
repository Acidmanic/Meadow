using System.ComponentModel.DataAnnotations;
using Meadow.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    public class PropertyTypeDal
    {
        public string Name { get; set; }
        
        public string Value { get; set; }
        
        [Key]
        [UniqueField]
        public long Id { get; set; }
    }
}