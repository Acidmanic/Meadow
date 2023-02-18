using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class DeleteByIdMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "DeleteById";


        protected override Dictionary<CommonSnippets, CodeGenerateBehavior> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, CodeGenerateBehavior>
            {
                { CommonSnippets.DeleteProcedure ,CodeGenerateBehavior.UseById}
            };
        }
    }
}