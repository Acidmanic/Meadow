using System;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models
{
    public class Category
    {
        [UniqueMember]
        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
    }
}