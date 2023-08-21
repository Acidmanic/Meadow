using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class EventSourcingMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "EventStream";


        protected override Dictionary<CommonSnippets, SnippetInstantiationInstruction> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, SnippetInstantiationInstruction>
            {
                { CommonSnippets.EventSteamScript ,CodeGenerateBehavior.UseIdAgnostic}
            };
        }
    }
}