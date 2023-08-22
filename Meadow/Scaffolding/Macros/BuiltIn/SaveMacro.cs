using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn
{
    public class SaveMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "Save";

        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.SaveProcedure).BehaviorUseIdAgnostic();
        }
    }
}