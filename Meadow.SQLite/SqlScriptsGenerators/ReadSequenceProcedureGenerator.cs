using System;
using System.Collections.Generic;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class ReadSequenceProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        public bool ById { get; }

        public int Top { get; }

        public bool OrderAscending { get; }

        private ProcessedType ProcessedType { get; }

        public ReadSequenceProcedureGenerator(Type type, bool byId, int top, bool orderAscending) : base(
            new SqLiteTypeNameMapper())
        {
            ById = byId;
            Top = top;
            OrderAscending = orderAscending;

            ProcessedType = Process(type);
        }


        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParametersDeclaration = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyOrderClause = GenerateKey();
        private readonly string _keyTopClause = GenerateKey();

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

            replacementList.Add(_keyOrderClause, OrderAscending ? " ORDER BY ROWID ASC" : "ORDER BY ROWID DESC");

            replacementList.Add(_keyTopClause, Top > 0 ? $" LIMIT {Top}" : "");
        }

        protected override string Template => $@"
CREATE PROCEDURE {_keyProcedureName}{_keyParametersDeclaration} AS
    SELECT * FROM {_keyTableName}{_keyWhereClause}{_keyOrderClause}{_keyTopClause}
GO
".Trim();
    }
}