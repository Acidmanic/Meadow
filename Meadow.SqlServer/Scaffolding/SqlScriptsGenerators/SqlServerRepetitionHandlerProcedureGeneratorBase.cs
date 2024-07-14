using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public abstract class SqlServerRepetitionHandlerProcedureGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {
        protected readonly string KeyCreationHeader = GenerateKey();
        protected readonly string KeyCreationHeaderFullTree = GenerateKey();
        protected readonly string KeyProcedureName = GenerateKey();
        protected readonly string KeyProcedureNameFullTree = GenerateKey();

        protected SqlServerRepetitionHandlerProcedureGeneratorBase(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution()
            {
                SqlExpressionTranslator = new SqlServerExpressionTranslator(){ Configuration = construction.MeadowConfiguration },
                TypeNameMapper = new SqlDbTypeNameMapper()
            })
        {
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(KeyCreationHeader, GetCreationHeader());
            replacementList.Add(KeyProcedureName, GetProcedureName(false));
            replacementList.Add(KeyProcedureNameFullTree, GetProcedureName(true));

            AddBodyReplacements(replacementList);
        }


        protected abstract string GetProcedureName(bool fullTree);
        protected abstract void AddBodyReplacements(Dictionary<string, string> replacementList);

        private string GetCreationHeader()
        {
            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                return "CREATE OR ALTER PROCEDURE";
            }

            return "CREATE PROCEDURE";
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            base.DeclareUnSupportedFeatures(declaration);

            declaration.NotSupported(RepetitionHandling.Skip);
        }

        protected abstract override string Template { get; }
    }
}