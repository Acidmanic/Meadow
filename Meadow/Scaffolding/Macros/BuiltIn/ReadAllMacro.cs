using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class ReadAllMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "ReadAll";


        protected override Dictionary<CommonSnippets, CodeGenerateBehavior> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, CodeGenerateBehavior>
            {
                { CommonSnippets.ReadProcedure ,CodeGenerateBehavior.UseAll}
            };
        }
    }
}