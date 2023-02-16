using Meadow.Scaffolding.Macros;
using Meadow.SqlServer.Scaffolding.SqlScriptsGenerators;
using Meadow.SqlServer.SqlScriptsGenerators;

namespace Meadow.SqlServer.Scaffolding.Macros
{
    public class InsertProcedureMacro:MacroBase
    {
        public override string Name => "InsertProcedure";

        public override string GenerateCode(params string[] arguments)
        {
            var type = GrabTypeArgument(arguments, 0);
            
            var insertGenerator = new InsertProcedureGenerator(type);

            var code = insertGenerator.Generate().Text;

            return code;
        }
    }
}