using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.SqlScriptsGenerators;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class CommentSectionTitleGenerator : ICodeGenerator
    {
        public CommentSectionTitleGenerator(string title)
        {
            Title = title;
        }

        private string Title { get; }

        public Code Generate()
        {
            return new Code
            {
                Name = "Comment",
                Text = SqlSingleLineGenerator.LineString +
                       SqlSingleLineGenerator.LineString +
                       "--\t\t\t\t\t\t\t" + Title + "\n" +
                       SqlSingleLineGenerator.LineString
            };
        }
    }
}