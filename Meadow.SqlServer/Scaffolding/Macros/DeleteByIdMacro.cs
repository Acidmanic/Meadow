using Meadow.Scaffolding.Macros;
using Meadow.SqlServer.Scaffolding.SqlScriptsGenerators;

namespace Meadow.SqlServer.Scaffolding.Macros
{
    public class DeleteByIdMacro:MacroBase
    {
        public override string Name { get; } = "DeleteById";
        
        public override string GenerateCode(params string[] arguments)
        {
            var type = GrabTypeArgument(arguments, 0);

            var reader = new DeleteProcedureGenerator(type, false);

            var code = reader.Generate().Text;

            return code;
        }
    }
}