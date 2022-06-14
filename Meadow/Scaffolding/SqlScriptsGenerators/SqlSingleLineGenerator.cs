using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public class SqlSingleLineGenerator:ICodeGenerator
    {
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
    }
}