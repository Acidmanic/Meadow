using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn
{
    public class InsertMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "Insert";


        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.InsertProcedure).BehaviorUseIdAgnostic();
        }
    }
}