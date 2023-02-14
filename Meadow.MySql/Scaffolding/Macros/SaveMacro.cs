using Meadow.MySql.Scaffolding.MySqlScriptGenerators;
using Meadow.Scaffolding.Macros;

namespace Meadow.MySql.Scaffolding.Macros
{
    public class SaveMacro:MacroBase
    {
        public override string Name { get; } = "Save";
        public override string GenerateCode(params string[] arguments)
        {
            var type = GrabTypeArgument(arguments, 0);

            var generator = new SaveProcedureGenerator(type);

            var code = generator.Generate().Text;

            return code;
        }
    }
}