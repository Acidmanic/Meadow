using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class DeleteAllMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "DeleteAll";



        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.DeleteProcedure).BehaviorUseAll();
        }
    }
}