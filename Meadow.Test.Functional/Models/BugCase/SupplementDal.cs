using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    public class SupplementDal
    {
        [UniqueMember] [AutoValuedMember] public long Id { get; set; }

        public ProductClassDal UniqueProductClass { get; set; }

        public long UniqueProductClassId { get; set; }

        public double Stock { get; set; }

        public int ValidDate { get; set; }

        public double MinVolume { get; set; }

        public double Price { get; set; }

        public bool AutoBid { get; set; }

        public List<StrategyParameterDal> Parameters { get; set; }

        public long AutobidStrategyId { get; set; }
        
        public AutobidStrategyDal AutobidStrategy { get; set; }
        
        
    }
}