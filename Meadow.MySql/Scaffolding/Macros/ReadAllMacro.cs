using Meadow.MySql.Scaffolding.MySqlScriptGenerators;
using Meadow.Scaffolding.Macros;

namespace Meadow.MySql.Scaffolding.Macros
{
    public class ReadAllMacro:MacroBase
    {
        public override string Name { get; } = "ReadAll";
        
        public override string GenerateCode(params string[] arguments)
        {
            var type = GrabTypeArgument(arguments, 0);

            var reader = new ReadSequenceProcedureGenerator(type, true, 0, true);

            var code = reader.Generate().Text;

            return code;
        }
    }
}