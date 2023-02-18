using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class SaveMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "Save";


        protected override Dictionary<CommonSnippets, CodeGenerateBehavior> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, CodeGenerateBehavior>
            {
                { CommonSnippets.SaveProcedure ,CodeGenerateBehavior.UseIdAgnostic}
            };
        }
    }
}