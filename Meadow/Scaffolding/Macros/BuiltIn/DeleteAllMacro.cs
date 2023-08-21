using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class DeleteAllMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "DeleteAll";


        protected override Dictionary<CommonSnippets, SnippetInstantiationInstruction> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, SnippetInstantiationInstruction>
            {
                { CommonSnippets.DeleteProcedure ,CodeGenerateBehavior.UseAll}
            };
        }
    }
}