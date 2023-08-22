using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.Macros.BuiltIn
{
    public class ReadByIdMacro : BuiltinMacroBase
    {
        public override string Name { get; } = "ReadById";

        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.ReadProcedure).BehaviorUseById();
        }
    }
}