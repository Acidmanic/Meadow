using Meadow.MySql.Scaffolding.MySqlScriptGenerators;
using Meadow.Scaffolding.Macros;

namespace Meadow.MySql.Scaffolding.Macros
{
    public class TableMacro:MacroBase
    {
        public override string Name { get; } = "Table";
        
        public override string GenerateCode(params string[] arguments)
        {
            var type = GrabTypeArgument(arguments, 0);
            
            var insertGenerator = new TableScriptGenerator(type);

            var code = insertGenerator.Generate().Text;

            return code;
        }
    }
}