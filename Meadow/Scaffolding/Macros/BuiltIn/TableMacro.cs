using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn
{
    public class TableMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "Table";


        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.CreateTable).BehaviorUseIdAgnostic();
        }
    }
}