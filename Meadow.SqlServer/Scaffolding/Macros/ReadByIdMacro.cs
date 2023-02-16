using Meadow.Scaffolding.Macros;
using Meadow.SqlServer.Scaffolding.SqlScriptsGenerators;
using Meadow.SqlServer.SqlScriptsGenerators;

namespace Meadow.SqlServer.Scaffolding.Macros
{
    public class ReadByIdMacro:MacroBase
    {
        public override string Name { get; } = "ReadById";
        
        public override string GenerateCode(params string[] arguments)
        {
            var type = GrabTypeArgument(arguments, 0);

            var reader = new ReadProcedureGenerator(type, true);

            var code = reader.Generate().Text;

            return code;
        }
    }
}