using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn
{
    public class DeleteByIdMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "DeleteById";

        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.DeleteProcedure).BehaviorUseById();
        }
    }
}