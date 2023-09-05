using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewCodeGenerator : SqlSnippetFullTreeViewGeneratorBase
    {
        public FullTreeViewCodeGenerator(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(new PostgreDbTypeNameMapper(), construction, configurations)
        {
        }

        protected override string GetCreationHeader()
        {
            var creationHeader = "create view";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                creationHeader = "create or replace view";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                LogUnSupportedRepetitionHandling("Postgre", "Views", RepetitionHandling.Skip);
            }

            return creationHeader;
        }

        protected override string LeadingTemplateText()
        {
            return Split;
        }

        protected override string TaleTemplateText()
        {
            return Split;
        }

        protected override bool DoubleQuoteIdentifiers => true;

        protected override string AliasQuote => "\"";
    }
}