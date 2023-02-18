using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class UpdateMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "Update";


        protected override Dictionary<CommonSnippets, CodeGenerateBehavior> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, CodeGenerateBehavior>
            {
                { CommonSnippets.UpdateProcedure ,CodeGenerateBehavior.UseIdAgnostic}
            };
        }
    }
}