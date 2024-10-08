using System;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewCodeGenerator : SqlSnippetFullTreeViewGeneratorBase
    {
        public FullTreeViewCodeGenerator(
            SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution()
            {
                SqlTranslator = new MySqlTranslator(construction.MeadowConfiguration),
                TypeNameMapper = new MySqlDbTypeNameMapper()
            })
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            declaration.NotSupported(RepetitionHandling.Skip);
        }

        protected override string GetCreationHeader()
        {
            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                return "DROP VIEW IF EXISTS " + GetViewName() + ";" +
                       "\nCREATE VIEW";
            }

            return "CREATE VIEW";
        }
    }
}