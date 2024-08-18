using System.Collections.Generic;
using System.Linq;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Newtonsoft.Json.Serialization;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FindPaged)]
    public class FindPagedSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public FindPagedSnippetGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution()
            {
                SqlExpressionTranslator = new SqLiteExpressionTranslator(construction.MeadowConfiguration),
                TypeNameMapper = new SqLiteTypeNameMapper()
            })
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            base.DeclareUnSupportedFeatures(declaration);

            declaration.NotSupportedRepetitionHandling();
            declaration.NotSupportedDbObjectNameOverriding();
        }

        private readonly string _keyFindPagedProcedureName = GenerateKey();
        private readonly string _keyFindPagedProcedureNameFullTree = GenerateKey();

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeView = GenerateKey();

        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldType = GenerateKey();
        private readonly string _keyIdFieldNameFullTree = GenerateKey();

        private readonly string _keyRemoveExpiredFilterProcedure = GenerateKey();
        private readonly string _keyFilterResultsTableName = GenerateKey();

        private readonly string _keyIndexProcedureName = GenerateKey();
        private readonly string _keySearchIndexTableName = GenerateKey();
        
        private readonly string _keyEntityFilterSegment = GenerateKey();
        
        private readonly string _keyColumns = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyFindPagedProcedureName,
                ProcessedType.NameConvention.FindPagedProcedureName);
            
            replacementList.Add(_keyFindPagedProcedureNameFullTree,
                ProcessedType.NameConvention.FindPagedProcedureNameFullTree);
            
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);
            
            replacementList.Add(_keyFullTreeView, ProcessedType.NameConvention.FullTreeViewName);

            replacementList.Add(_keyIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name : "[NO-ID-FIELD]");
            
            replacementList.Add(_keyIdFieldType,
                ProcessedType.HasId ? ProcessedType.IdParameter.Type : "[NO-ID-FIELD]");

            replacementList.Add(_keyIdFieldNameFullTree,
                ProcessedType.HasId ? ProcessedType.IdParameterFullTree.Name : "[NO-ID-FIELD]");

            replacementList.Add(_keyRemoveExpiredFilterProcedure,
                ProcessedType.NameConvention.RemoveExpiredFilterResultsProcedureName);

            replacementList.Add(_keyFilterResultsTableName, ProcessedType.NameConvention.FilterResultsTableName);

            replacementList.Add(_keyIndexProcedureName, ProcessedType.NameConvention.IndexEntityProcedureName);
            replacementList.Add(_keySearchIndexTableName, ProcessedType.NameConvention.SearchIndexTableName);
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            
            var entityFilterSegment = entityFilterExpression.Success ? $" AND {entityFilterExpression.Value} " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);

            var insertParameters = ProcessedType.GetInsertParameters();
            
            var columns = string.Join(',', insertParameters.Select(p => p.Name));

            replacementList.Add(_keyColumns, columns);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFindPagedProcedureName}(
                                            @Offset INTEGER,
                                            @Size INTEGER,
                                            @FilterExpression TEXT,
                                            @SearchExpression TEXT,
                                            @OrderExpression TEXT)
AS
    SELECT {_keyColumns} FROM {_keyTableName}
    LEFT JOIN {_keySearchIndexTableName} ON {_keyTableName}.{_keyIdFieldName}={_keySearchIndexTableName}.ResultId
    WHERE (&@FilterExpression) AND (&@SearchExpression){_keyEntityFilterSegment}
    ORDER BY &@OrderExpression LIMIT @Offset,@Size;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFindPagedProcedureNameFullTree}(
                                            @Offset INTEGER,
                                            @Size INTEGER,
                                            @FilterExpression TEXT,
                                            @SearchExpression TEXT,
                                            @OrderExpression TEXT)
AS
    SELECT {_keyFullTreeView}.* FROM {_keyFullTreeView}
    LEFT JOIN {_keySearchIndexTableName} ON {_keyFullTreeView}.{_keyIdFieldNameFullTree}={_keySearchIndexTableName}.ResultId
    WHERE (&@FilterExpression) AND (&@SearchExpression)
    ORDER BY &@OrderExpression LIMIT @Offset,@Size;
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}