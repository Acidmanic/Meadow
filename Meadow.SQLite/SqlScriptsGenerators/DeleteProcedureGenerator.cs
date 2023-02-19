using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.DeleteProcedure)]
    public class DeleteProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        public bool ById { get; }

        private ProcessedType ProcessedType { get; }

        public DeleteProcedureGenerator(Type type, bool byId) : base(new SqLiteTypeNameMapper())
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
            replacementList.Add(_keyProcedureName, ById
                ? ProcessedType.NameConvention.DeleteByIdProcedureName
                : ProcessedType.NameConvention.DeleteAllProcedureName);

            var parameters = ById ? $"({ParameterNameTypeJoint(ProcessedType.IdParameter, "@")})" : "";

            replacementList.Add(_keyParametersDeclaration, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            var whereClause = ById
                ? $" WHERE {ProcessedType.NameConvention.TableName}.{ProcessedType.IdParameter.Name} = @{ProcessedType.IdParameter.Name}"
                : "";

            replacementList.Add(_keyWhereClause, whereClause);
        }

        protected override string Template => $@"
CREATE PROCEDURE {_keyProcedureName}{_keyParametersDeclaration} AS

    PRAGMA temp_store = 2; /* 2 means use in-memory */
    CREATE TEMP TABLE _Existing(Count INTEGER);
    INSERT INTO _Existing (Count) SELECT COUNT(*) FROM {_keyTableName};
    DELETE FROM {_keyTableName}{_keyWhereClause};
    INSERT INTO _Existing (Count) SELECT COUNT(*) FROM {_keyTableName};
    SELECT CASE WHEN Count(DISTINCT Count)=2 THEN CAST(1 as bit) ELSE CAST(0 as bit) 
                END AS Success
                FROM _Existing;
    DROP TABLE _Existing;
GO
";
    }
}