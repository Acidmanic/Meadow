using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.BugCase
{
    [OwnerName("AutobidStrategies")]
    public class AutobidStrategyDal
    {
        [AutoValuedMember] [UniqueMember] public long Id { get; set; }

        public string Name { get; set; }

        [UniqueMember] public string DefinitionUniqueId { get; set; }

        public List<StrategyParameterDescriptorDal> ParameterDescriptors { get; set; }
    }
}