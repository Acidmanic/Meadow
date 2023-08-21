using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Macros.BuiltIn
{

    public class TableMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "Table";


        protected override Dictionary<CommonSnippets, SnippetInstantiationInstruction> GetAssemblyBehavior()
        {
            return new Dictionary<CommonSnippets, SnippetInstantiationInstruction>
            {
                { CommonSnippets.CreateTable ,CodeGenerateBehavior.UseIdAgnostic}
            };
        }
    }
}