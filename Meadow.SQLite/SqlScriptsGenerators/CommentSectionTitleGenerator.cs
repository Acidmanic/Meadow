using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class CommentSectionTitleGenerator : ICodeGenerator
    {
        public CommentSectionTitleGenerator(string title)
        {
            Title = title;
        }

        private string Title { get; }

        public RepetitionHandling RepetitionHandling { get; set; } = RepetitionHandling.Create;

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