using Acidmanic.Utilities.Filtering.Models;
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
            .OverrideEntityType<FilterResult>()
            .OnRepetitionSkipNewOne();
        builder.Add(CommonSnippets.FilteringProcedures).BehaviorUseIdAgnostic();
        builder.Add(CommonSnippets.DataBound).BehaviorUseIdAgnostic();
    }
}