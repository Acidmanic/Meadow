using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
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