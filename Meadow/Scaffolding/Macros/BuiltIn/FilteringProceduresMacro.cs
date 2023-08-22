using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class FilteringProceduresMacro : BuiltinMacroBase
{
    public override string Name => "FilteringProcedures";

    protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
    {
        builder.Add(CommonSnippets.FilteringProcedures).BehaviorUseIdAgnostic();
    }
}