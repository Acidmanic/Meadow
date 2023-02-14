namespace Meadow.SqlServer.SqlScriptsGenerators
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