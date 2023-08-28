using System;
using Meadow.Scaffolding.Attributes;
using Meadow.Sql;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewGenerator : SqlFullTreeViewGeneratorBase
    {
        public FullTreeViewGenerator(Type type) : base(type, new SqlDbTypeNameMapper())
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
    }
}