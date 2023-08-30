using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewGenerator : SqlFullTreeViewGeneratorBase
    {
        public FullTreeViewGenerator(Type type, MeadowConfiguration configuration)
            : base(type, configuration, new SqlDbTypeNameMapper())
        {
        }

        protected override string GetCreationHeader()
        {
            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                return "CREATE OR ALTER VIEW";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                LogUnSupportedRepetitionHandling("SqlServer", "Views", RepetitionHandling.Skip);
            }

            return "CREATE VIEW";
        }

        protected override string LeadingTemplateText()
        {
            return Split;
        }

        protected override string TaleTemplateText()
        {
            return Split;
        }
    }
}