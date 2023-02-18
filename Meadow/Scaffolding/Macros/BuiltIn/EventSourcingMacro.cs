using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class EventSourcingMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "EventStream";


        protected override Dictionary<CommonSnippets, CodeGenerateBehavior> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, CodeGenerateBehavior>
            {
                { CommonSnippets.EventSteamScript ,CodeGenerateBehavior.UseIdAgnostic}
            };
        }
    }
}