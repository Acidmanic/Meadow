using System;
using System.Collections.Generic;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.SqlScriptsGenerators
{
    public class ReadSequenceProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        public int Count { get; }

        public bool OrderAscending { get; }

        private ProcessedType ProcessedType { get; }

        public ReadSequenceProcedureGenerator(Type type, int count, bool orderAscending) : base(
            new SqlDbTypeNameMapper())
        {
            Count = count;
            OrderAscending = orderAscending;
            ProcessedType = Process(type);
        }

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyOrder = GenerateKey();


        private string GetProcedureName()
        {
            if (IsDatabaseObjectNameForced)
            {
                return ForcedDatabaseObjectName;
            }

            return OrderAscending
                ? ProcessedType.NameConvention.SelectFirstProcedureName
                : ProcessedType.NameConvention.SelectLastProcedureName;
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, GetProcedureName());

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyOrder, OrderAscending ? "ASC" : "DESC");
        }

        protected override string Template => $@"
CREATE PROCEDURE {_keyProcedureName} AS

	SELECT TOP {Count} * FROM {_keyTableName} ORDER BY {_keyIdFieldName} {_keyOrder};

GO
";
    }
}