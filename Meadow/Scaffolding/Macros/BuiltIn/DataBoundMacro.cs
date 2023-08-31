using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class DataBoundMacro:BuiltinMacroBase
{
    public override string Name => "DataBound";
    protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
    {
        builder.Add(CommonSnippets.DataBound).BehaviorUseIdAgnostic();
    }
}