using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class FullTreeViewMacro:BuiltinMacroBase
{
    public override string Name => "FullTreeView";
    protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
    {
        builder.Add(CommonSnippets.FullTreeView).BehaviorUseIdAgnostic();
    }
}