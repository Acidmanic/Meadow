using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewGenerator : SqlFullTreeViewGeneratorBase
    {
        public FullTreeViewGenerator(Type type,MeadowConfiguration configuration) 
            : base(type,configuration, new PostgreDbTypeNameMapper())
        {
        }

        private static readonly string Split = @"
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------"
            .Trim();

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