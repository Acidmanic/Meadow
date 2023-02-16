using Meadow.Scaffolding.Macros;
using Meadow.SqlServer.Scaffolding.SqlScriptsGenerators;
using Meadow.SqlServer.SqlScriptsGenerators;

namespace Meadow.SqlServer.Scaffolding.Macros
{
    public class DeleteAllMacro:MacroBase
    {
        public override string Name { get; } = "DeleteAll";
        
        public override string GenerateCode(params string[] arguments)
        {
            var type = GrabTypeArgument(arguments, 0);

            var reader = new DeleteProcedureGenerator(type, true);

            var code = reader.Generate().Text;

            return code;
        }
    }
}