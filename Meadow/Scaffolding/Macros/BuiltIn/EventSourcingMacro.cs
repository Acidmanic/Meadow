using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class EventSourcingMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "EventStream";
        
        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.EventStreamScript).BehaviorUseIdAgnostic();
        }
    }
}