using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class ReadSequenceProcedureGenerator : SqlServerByTemplateCodeGeneratorBase
    {
        public int Count { get; }

        public bool OrderAscending { get; }


        public ReadSequenceProcedureGenerator(Type type, MeadowConfiguration configuration, int count,
            bool orderAscending)
            : base(type, configuration)
        {
            Count = count;
            OrderAscending = orderAscending;
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyOrder = GenerateKey();


        protected override string GetProcedureName()
        {
            if (IsDatabaseObjectNameForced)
            {
                return ForcedDatabaseObjectName;
            }

            return OrderAscending
                ? ProcessedType.NameConvention.SelectFirstProcedureName
                : ProcessedType.NameConvention.SelectLastProcedureName;
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyOrder, OrderAscending ? "ASC" : "DESC");
        }

        protected override string Template => $@"
{KeyCreationHeader} {KeyProcedureName} AS

	SELECT TOP {Count} * FROM {_keyTableName} ORDER BY {_keyIdFieldName} {_keyOrder};

GO
";
    }
}