using Meadow.Scaffolding.Macros;
using Meadow.SqlServer.SqlScriptsGenerators;

namespace Meadow.SqlServer.Scaffolding.Macros
{
    public class UpdateMacro:MacroBase
    {
        public override string Name { get; } = "Update";
        public override string GenerateCode(params string[] arguments)
        {
            var type = GrabTypeArgument(arguments, 0);

            var generator = new UpdateProcedureGenerator(type);

            var code = generator.Generate().Text;

            return code;
        }
    }
}