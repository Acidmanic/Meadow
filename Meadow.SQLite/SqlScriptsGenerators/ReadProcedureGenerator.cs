using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        public bool ById { get; }

        private ProcessedType ProcessedType { get; }

        public ReadProcedureGenerator(Type type, bool byId) : base(new SqLiteTypeNameMapper())
        {
            ById = byId;

            ProcessedType = Process(type);
        }


        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParametersDeclaration = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName,
                ById
                    ? ProcessedType.NameConvention.SelectByIdProcedureName
                    : ProcessedType.NameConvention.SelectAllProcedureName);

            replacementList.Add(_keyParametersDeclaration, ById ? 
                $"({ParameterNameTypeJoint(ProcessedType.IdParameter,"@")})" : "");

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var whereClause =
                ById ? $" WHERE {ProcessedType.IdParameter?.Name} = @{ProcessedType.IdParameter?.Name}" : "";

            replacementList.Add(_keyWhereClause, whereClause);
        }

        protected override string Template => $@"
CREATE PROCEDURE {_keyProcedureName}{_keyParametersDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause};
GO
".Trim();
    }
}