using System;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class SnippetFullTreeViewGenerator : SqlSnippetFullTreeViewGeneratorBase
    {
        public SnippetFullTreeViewGenerator(Type type, MeadowConfiguration configuration)
            : base(type, configuration, new MySqlDbTypeNameMapper())
        {
        }

        protected override string GetCreationHeader()
        {
            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                LogUnSupportedRepetitionHandling("MySql", "View", RepetitionHandling.Skip);
            }

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                return "DROP VIEW IF EXISTS " + GetViewName() + ";" +
                       "\nCREATE VIEW";
            }

            return "CREATE VIEW";
        }
    }
}