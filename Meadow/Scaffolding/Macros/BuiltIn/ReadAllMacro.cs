using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn
{
    public class ReadAllMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "ReadAll";


        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.ReadProcedure).BehaviorUseAll();
        }
    }
}