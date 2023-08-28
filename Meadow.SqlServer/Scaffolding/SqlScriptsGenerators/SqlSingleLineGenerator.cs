using Meadow.Configuration;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class SqlSingleLineGenerator:ICodeGenerator
    {
        
        public RepetitionHandling RepetitionHandling { get; set; } = RepetitionHandling.Create;
        
        public static string LineString =
            "--------------------------------------------------------------------------------------------\n";
        public Code Generate()
        {
            return new Code
            {
                Name = "Line",
                Text = LineString
            };
        }

        public MeadowConfiguration Configuration { get; set; }
    }
}