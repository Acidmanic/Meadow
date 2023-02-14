using Meadow.MySql.Scaffolding.MySqlScriptGenerators;
using Meadow.Scaffolding.Macros;

namespace Meadow.MySql.Scaffolding.Macros
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