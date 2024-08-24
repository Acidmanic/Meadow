using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.SnippetComposed;

namespace Meadow.Scaffolding.Macros.BuiltIn
{
    public class SnippetComposedWipAllMacro : SnippetComposedMacroBase
    {
        public override string Name { get; } = "WipAll";

        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.CreateTable);
            builder.Add(CommonSnippets.SaveProcedure).BehaviorUseIdAgnostic();
        }
    }
}