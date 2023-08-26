using System;
using System.Collections.Generic;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    public abstract class PostgreByTemplateProcedureGeneratorBase : ByTemplateSqlGeneratorBase
    {
        protected Type EntityType { get; }
        protected ProcessedType ProcessedType { get; }

        protected readonly string KeyCreationHeader = GenerateKey();
        protected readonly string KeyProcedureName = GenerateKey();

        public PostgreByTemplateProcedureGeneratorBase(Type entityType) : base(new PostgreDbTypeNameMapper())
        {
            EntityType = entityType;
            ProcessedType = Process(entityType);
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