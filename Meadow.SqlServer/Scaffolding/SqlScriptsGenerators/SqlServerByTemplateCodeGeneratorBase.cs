using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public abstract class SqlServerByTemplateCodeGeneratorBase : ByTemplateSqlGeneratorBase
    {
        protected Type EntityType { get; }

        protected ProcessedType ProcessedType { get; }


        protected readonly string KeyCreationHeader = GenerateKey();
        protected readonly string KeyProcedureName = GenerateKey();

        public SqlServerByTemplateCodeGeneratorBase(Type entityType, MeadowConfiguration configuration)
            : base(new SqlDbTypeNameMapper(), configuration)
        {
            EntityType = entityType;
            ProcessedType = Process(EntityType);
        }


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(KeyCreationHeader, GetCreationHeader());
            replacementList.Add(KeyProcedureName, GetProcedureName());

            AddBodyReplacements(replacementList);
        }


        protected abstract string GetProcedureName();
        protected abstract void AddBodyReplacements(Dictionary<string, string> replacementList);

        private string GetCreationHeader()
        {
            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                return "CREATE OR ALTER PROCEDURE";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                LogUnSupportedRepetitionHandling("SqlServer", "Procedures", RepetitionHandling.Skip);
            }

            return "CREATE PROCEDURE";
        }

        protected abstract override string Template { get; }
    }
}