using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public abstract class SqLiteByTemplateProcedureSnippetGeneratorBase : ByTemplateSqlSnippetGeneratorBase
    {
        protected Type EntityType { get; }
        protected ProcessedType ProcessedType { get; }


        protected readonly string KeyHeaderCreation = GenerateKey();

        public SqLiteByTemplateProcedureSnippetGeneratorBase(Type type,MeadowConfiguration configuration)
            : base(new SqLiteTypeNameMapper(),configuration)
        {
            EntityType = type;
            ProcessedType = Process(EntityType);
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