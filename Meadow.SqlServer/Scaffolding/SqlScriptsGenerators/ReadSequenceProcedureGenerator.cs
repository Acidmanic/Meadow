using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class ReadSequenceProcedureGenerator : SqlServerRepetitionHandlerProcedureGeneratorBase
    {
        public int Count { get; }

        public bool OrderAscending { get; }


        public ReadSequenceProcedureGenerator(Type type, MeadowConfiguration configuration, int count,
            bool orderAscending)
            : base(new SnippetConstruction
            {EntityType = type,MeadowConfiguration = configuration}, 
                SnippetConfigurations.Default())
        {
            Count = count;
            OrderAscending = orderAscending;
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyOrder = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();


        protected override string GetProcedureName(bool fullTree)
        {
            return ProvideDbObjectNameSupportingOverriding(() => OrderAscending
                ? ProcessedType.NameConvention.SelectFirstProcedureName
                : ProcessedType.NameConvention.SelectLastProcedureName);
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyOrder, OrderAscending ? "ASC" : "DESC");
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            
            var entityFilterSegment = entityFilterExpression.Success ? $"WHERE ({entityFilterExpression.Value}) " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
        }

        protected override string Template => $@"
{KeyCreationHeader} {KeyProcedureName} AS

	SELECT TOP {Count} * FROM {_keyEntityFilterSegment}{_keyTableName} ORDER BY {_keyIdFieldName} {_keyOrder};

GO
";
    }
}