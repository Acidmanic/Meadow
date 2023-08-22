using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class SqlSplitterGenerator : ICodeGenerator
    {
        public RepetitionHandling RepetitionHandling { get; set; } = RepetitionHandling.Create;

        public Code Generate()
        {
            return new Code
            {
                Name = "Splitter",
                Text = "--SPLIT\n"
            };
        }
    }
}