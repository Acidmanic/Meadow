using Acidmanic.Utilities.Filtering.Models;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class FilterResultsTableMacro : BuiltinMacroBase
{
    public override string Name => "FilterResultsTable";

    protected override bool TakesTypeArgument => false;


    protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
    {
        builder.Add(CommonSnippets.FilterResultTable).BehaviorUseIdAgnostic();
    }
}