using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class FindPagedMacro : BuiltinMacroBase
{
    public override string Name => "FindPaged";

    protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
    {
        // builder.Add(CommonSnippets.CreateTable)
        //     .BehaviorUseIdAgnostic()
        //     .OverrideEntityTypeBySearchIndex()
        //     .OverrideDbObjectNameToSearchIndexTableName();
        builder.Add(CommonSnippets.FindPaged).BehaviorUseIdAgnostic();
        builder.Add(CommonSnippets.DataBound).BehaviorUseIdAgnostic();
    }
}