using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class FilteringMacro : BuiltinMacroBase
{
    public override string Name => "Filtering";

    protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
    {
        builder.Add(CommonSnippets.CreateTable)
            .BehaviorUseIdAgnostic()
            .OverrideEntityTypeByFilterResults()
            .OverrideDbObjectNameToFilterResultsTableName();
        builder.Add(CommonSnippets.CreateTable)
            .BehaviorUseIdAgnostic()
            .OverrideEntityTypeBySearchIndex()
            .OverrideDbObjectNameToSearchIndexTableName();
        builder.Add(CommonSnippets.FilteringProcedures).BehaviorUseIdAgnostic();
        builder.Add(CommonSnippets.DataBound).BehaviorUseIdAgnostic();
    }
}