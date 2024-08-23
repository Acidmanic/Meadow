using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.SnippetComposed;

namespace Meadow.Scaffolding.Macros.BuiltIn
{
    public class SnippetComposedSaveMacro : SnippetComposedMacroBase
    {
        public override string Name { get; } = "ScSave";

        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.SaveProcedure).BehaviorUseIdAgnostic();
        }
    }
}