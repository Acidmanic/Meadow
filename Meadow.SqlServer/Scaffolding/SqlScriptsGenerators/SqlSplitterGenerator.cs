using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
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