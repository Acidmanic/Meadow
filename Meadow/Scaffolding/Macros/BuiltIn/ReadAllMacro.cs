using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class ReadAllMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "ReadAll";


        protected override Dictionary<CommonSnippets, SnippetInstantiationInstruction> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, SnippetInstantiationInstruction>
            {
                { CommonSnippets.ReadProcedure ,CodeGenerateBehavior.UseAll}
            };
        }
    }
}