using Meadow.Scaffolding.Macros;
using Meadow.SqlServer.Scaffolding.SqlScriptsGenerators;
using Meadow.SqlServer.SqlScriptsGenerators;

namespace Meadow.SqlServer.Scaffolding.Macros
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