using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class SqlSplitterGenerator : ICodeGenerator
    {
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