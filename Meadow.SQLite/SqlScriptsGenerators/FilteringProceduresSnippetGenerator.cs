using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Newtonsoft.Json.Serialization;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FilteringProcedures)]
    public class FilteringProceduresSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public FilteringProceduresSnippetGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution()
            {
                SqlExpressionTranslator = new SqLiteExpressionTranslator(),
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

        private readonly string _keyFilterProcedureName = GenerateKey();
        private readonly string _keyFilterProcedureNameFullTree = GenerateKey();
        private readonly string _keyReadChunkProcedureName = GenerateKey();
        private readonly string _keyReadChunkProcedureNameFullTree = GenerateKey();

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeView = GenerateKey();

        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldType = GenerateKey();
        private readonly string _keyIdFieldNameFullTree = GenerateKey();

        private readonly string _keyRemoveExpiredFilterProcedure = GenerateKey();
        private readonly string _keyFilterResultsTableName = GenerateKey();

        private readonly string _keyIndexProcedureName = GenerateKey();
        private readonly string _keySearchIndexTableName = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyFilterProcedureName,
                ProcessedType.NameConvention.PerformFilterIfNeededProcedureName);
            replacementList.Add(_keyFilterProcedureNameFullTree,
                ProcessedType.NameConvention.PerformFilterIfNeededProcedureNameFullTree);

            replacementList.Add(_keyReadChunkProcedureName, ProcessedType.NameConvention.ReadChunkProcedureName);
            replacementList.Add(_keyReadChunkProcedureNameFullTree,
                ProcessedType.NameConvention.ReadChunkProcedureNameFullTree);

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
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyIndexProcedureName} (@ResultId {_keyIdFieldType},@IndexCorpus TEXT) AS
    UPDATE {_keySearchIndexTableName}  SET IndexCorpus=@IndexCorpus
        WHERE {_keySearchIndexTableName}.ResultId=@ResultId;
    INSERT INTO {_keySearchIndexTableName} (ResultId,IndexCorpus)
            SELECT @ResultId,@IndexCorpus WHERE NOT EXISTS(SELECT * FROM {_keySearchIndexTableName}
            WHERE {_keySearchIndexTableName}.ResultId=@ResultId);
    SELECT * FROM {_keySearchIndexTableName} WHERE ROWID=LAST_INSERT_ROWID();
    SELECT * FROM {_keySearchIndexTableName} WHERE {_keySearchIndexTableName}.ResultId=@ResultId OR ROWID = LAST_INSERT_ROWID() LIMIT 1;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyRemoveExpiredFilterProcedure}(@ExpirationTimeStamp INTEGER)
AS
    DELETE FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.ExpirationTimeStamp < @ExpirationTimeStamp;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterProcedureName}(@SearchId TEXT,
                                                  @ExpirationTimeStamp INTEGER,
                                                  @FilterExpression TEXT,
                                                  @SearchExpression TEXT,
                                                  @OrderExpression TEXT)
AS
    INSERT INTO {_keyFilterResultsTableName} (SearchId, ResultId, ExpirationTimeStamp) 
    SELECT @SearchId,{_keyTableName}.{_keyIdFieldName},@ExpirationTimeStamp FROM {_keyTableName}
    LEFT JOIN {_keySearchIndexTableName} ON {_keyTableName}.{_keyIdFieldName}={_keySearchIndexTableName}.ResultId
    WHERE (&@FilterExpression) AND (&@SearchExpression)
    AND IIF((select count(Id) from {_keyFilterResultsTableName} where {_keyFilterResultsTableName}.SearchId=@SearchId)>0,false,true)
    ORDER BY &@OrderExpression;

    SELECT Count(SearchId) 'Count', @SearchId 'SearchId' FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.SearchId=@SearchId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterProcedureNameFullTree}(@SearchId TEXT,
                                                  @ExpirationTimeStamp INTEGER,
                                                  @FilterExpression TEXT,
                                                  @SearchExpression TEXT,
                                                  @OrderExpression TEXT)
AS
    INSERT INTO {_keyFilterResultsTableName} (SearchId, ResultId, ExpirationTimeStamp) 
    SELECT @SearchId,{_keyFullTreeView}.{_keyIdFieldNameFullTree},@ExpirationTimeStamp FROM {_keyFullTreeView}
    LEFT JOIN {_keySearchIndexTableName} ON {_keyFullTreeView}.{_keyIdFieldNameFullTree}={_keySearchIndexTableName}.ResultId
    WHERE (&@FilterExpression) AND (&@SearchExpression) 
    AND IIF((select count(Id) from {_keyFilterResultsTableName} where {_keyFilterResultsTableName}.SearchId=@SearchId)>0,false,true)
    GROUP BY {_keyFullTreeView}.{_keyIdFieldNameFullTree}
    ORDER BY &@OrderExpression;
    
    SELECT Count(SearchId) 'Count', @SearchId 'SearchId' FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.SearchId=@SearchId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadChunkProcedureName}(@Offset INTEGER,
                                      @Size INTEGER,
                                      @SearchId TEXT)
AS
    SELECT {_keyTableName}.* FROM {_keyTableName} INNER JOIN {_keyFilterResultsTableName} ON {_keyTableName}.{_keyIdFieldName} = {_keyFilterResultsTableName}.ResultId
    WHERE {_keyFilterResultsTableName}.SearchId=@SearchId ORDER BY {_keyFilterResultsTableName}.Id LIMIT @Offset,@Size;  
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadChunkProcedureNameFullTree}(@Offset INTEGER,
                                      @Size INTEGER,
                                      @SearchId TEXT)
AS
    SELECT {_keyFullTreeView}.* FROM {_keyFullTreeView} 
        INNER JOIN (SELECT * FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.SearchId=@SearchId 
        ORDER BY {_keyFilterResultsTableName}.Id LIMIT @Offset,@Size) FR
    ON {_keyFullTreeView}.{_keyIdFieldNameFullTree} = FR.ResultId ;  
GO


-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}