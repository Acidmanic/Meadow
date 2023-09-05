using System;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewCodeGenerator : SqlSnippetFullTreeViewGeneratorBase
    {
        public FullTreeViewCodeGenerator(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(new SqlDbTypeNameMapper(), construction, configurations)
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