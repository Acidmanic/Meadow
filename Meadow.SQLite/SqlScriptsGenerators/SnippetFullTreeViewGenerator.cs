using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class SnippetFullTreeViewGenerator : SqlSnippetFullTreeViewGeneratorBase
    {
        public SnippetFullTreeViewGenerator(SnippetConstruction construction, SnippetConfigurations configurations) 
            : base( construction, configurations, new SnippetExecution()
            {
                SqlTranslator = new SqLiteTranslator(construction.MeadowConfiguration),
                TypeNameMapper = new SqLiteTypeNameMapper()
            })
        {
        }

        protected override string GetCreationHeader()
        {
            var creationHeader = "CREATE VIEW";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                creationHeader = "DROP VIEW IF EXISTS " + GetViewName() + ";" +
                                 "\nCREATE";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                creationHeader = "CREATE VIEW IF NOT EXISTS ";
            }

            return creationHeader;
        }

        protected override string TaleTemplateText()
        {
            return Split;
        }
    }
}