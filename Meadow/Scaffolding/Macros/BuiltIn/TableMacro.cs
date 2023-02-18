using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class TableMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "Table";


        protected override Dictionary<CommonSnippets, CodeGenerateBehavior> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, CodeGenerateBehavior>
            {
                { CommonSnippets.CreateTable ,CodeGenerateBehavior.UseIdAgnostic}
            };
        }
    }
}