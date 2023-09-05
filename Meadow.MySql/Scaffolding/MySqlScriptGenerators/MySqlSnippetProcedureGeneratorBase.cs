using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public abstract class MySqlSnippetProcedureGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {
        protected ProcessedType Processed { get; }

        protected readonly string KeyCreationHeader = GenerateKey();
        protected readonly string KeyProcedureName = GenerateKey();
        protected readonly string KeyCreationHeaderFullTree = GenerateKey();
        protected readonly string KeyProcedureNameFullTree = GenerateKey();

        protected Type EntityType { get; }

        public MySqlSnippetProcedureGeneratorBase(Type entityType, MeadowConfiguration configuration) : base(
            new MySqlDbTypeNameMapper(), configuration)
        {
            EntityType = entityType;
            Processed = Process(EntityType);
        }


        protected string GetCreationHeader(bool fullTree)
        {
            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                LogUnSupportedRepetitionHandling("MySql", "Procedures", RepetitionHandling.Skip);
            }

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