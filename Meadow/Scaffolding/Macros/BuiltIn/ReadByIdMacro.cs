using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class ReadByIdMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "ReadById";


        protected override Dictionary<CommonSnippets, CodeGenerateBehavior> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, CodeGenerateBehavior>
            {
                { CommonSnippets.ReadProcedure ,CodeGenerateBehavior.UseById}
            };
        }
    }
}