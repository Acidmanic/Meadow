using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn;

public class CrudMacro : BuiltinMacroBase
{
    public override string Name { get; } = "Crud";

    protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
    {
        builder.Add(CommonSnippets.CreateTable).BehaviorUseIdAgnostic();
        builder.Add(CommonSnippets.FullTreeView).BehaviorUseIdAgnostic();
        builder.Add(CommonSnippets.InsertProcedure).BehaviorUseIdAgnostic();
        builder.Add(CommonSnippets.ReadProcedure).BehaviorUseIdAware();
        builder.Add(CommonSnippets.DeleteProcedure).Behavior(CodeGenerateBehavior.UseById | CodeGenerateBehavior.UseAll);
        builder.Add(CommonSnippets.UpdateProcedure).BehaviorUseIdAgnostic();
        builder.Add(CommonSnippets.SaveProcedure).BehaviorUseIdAgnostic();
    }
}