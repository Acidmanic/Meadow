using System.Collections.Generic;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public abstract class MySqlRepetitionHandlerProcedureGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {
        protected readonly string KeyCreationHeader = GenerateKey();
        protected readonly string KeyProcedureName = GenerateKey();
        protected readonly string KeyCreationHeaderFullTree = GenerateKey();
        protected readonly string KeyProcedureNameFullTree = GenerateKey();
        
        protected MySqlRepetitionHandlerProcedureGeneratorBase(SnippetConstruction construction, SnippetConfigurations configurations) 
            : base(construction, configurations, new SnippetExecution
            {
                SqlExpressionTranslator = new MySqlExpressionTranslator(),
                TypeNameMapper = new MySqlDbTypeNameMapper()
            })
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            declaration.NotSupported(RepetitionHandling.Skip);
        }


        private string GetCreationHeader(bool fullTree)
        {
            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                return "DROP PROCEDURE IF EXISTS " + GetProcedureName(fullTree) + ";" +
                       "\nCREATE PROCEDURE";
            }

            return "CREATE PROCEDURE";
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(KeyProcedureName, GetProcedureName(false));

            replacementList.Add(KeyCreationHeader, GetCreationHeader(false));

            replacementList.Add(KeyProcedureNameFullTree, GetProcedureName(true));

            replacementList.Add(KeyCreationHeaderFullTree, GetCreationHeader(true));

            AddBodyReplacements(replacementList);
        }

        protected abstract string GetProcedureName(bool fullTree);

        protected abstract void AddBodyReplacements(Dictionary<string, string> replacementList);

        
    }
}