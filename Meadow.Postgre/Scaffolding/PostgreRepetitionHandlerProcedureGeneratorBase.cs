using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    public abstract class PostgreRepetitionHandlerProcedureGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {
        protected readonly string KeyCreationHeader = GenerateKey();
        protected readonly string KeyProcedureName = GenerateKey();

        protected PostgreRepetitionHandlerProcedureGeneratorBase(
            SnippetConstruction construction, SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution
            {
                SqlExpressionTranslator = new PostgreSqlExpressionTranslator(construction.MeadowConfiguration),
                TypeNameMapper = new PostgreDbTypeNameMapper()
            })
        {
        }


        protected abstract string GetProcedureName();
        protected abstract void AddBodyReplacements(Dictionary<string, string> replacementList);

        protected sealed override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var creationHeader = GetCreationHeader();

            replacementList.Add(KeyCreationHeader, creationHeader);

            replacementList.Add(KeyProcedureName, GetProcedureName());

            AddBodyReplacements(replacementList);
        }


        private string GetCreationHeader()
        {
            var creationHeader = "create";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                creationHeader = "create or replace";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                LogUnSupportedRepetitionHandling("Postgre", "Procedures", RepetitionHandling.Skip);
            }

            return creationHeader;
        }
    }
}