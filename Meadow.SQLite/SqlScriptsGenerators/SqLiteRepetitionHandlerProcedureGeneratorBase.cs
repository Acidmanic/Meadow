using System.Collections.Generic;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public abstract class SqLiteRepetitionHandlerProcedureGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {
        protected readonly string KeyHeaderCreation = GenerateKey();

        protected SqLiteRepetitionHandlerProcedureGeneratorBase(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution
            {
                SqlExpressionTranslator = new SqLiteExpressionTranslator(){ Configuration = construction.MeadowConfiguration },
                TypeNameMapper = new SqLiteTypeNameMapper()
            })
        {
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var creationHeader = "CREATE PROCEDURE";

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                creationHeader = "CREATE IF NOT EXISTS";
            }

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                creationHeader = "CREATE OR ALTER";
            }

            replacementList.Add(KeyHeaderCreation, creationHeader);

            AddBodyReplacements(replacementList);
        }

        protected abstract void AddBodyReplacements(Dictionary<string, string> replacementList);
    }
}