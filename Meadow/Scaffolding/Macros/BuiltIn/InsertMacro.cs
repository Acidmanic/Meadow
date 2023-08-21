using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class InsertMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "Insert";


        protected override Dictionary<CommonSnippets, SnippetInstantiationInstruction> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, SnippetInstantiationInstruction>
            {
                { CommonSnippets.InsertProcedure ,CodeGenerateBehavior.UseIdAgnostic}
            };
        }
    }
}