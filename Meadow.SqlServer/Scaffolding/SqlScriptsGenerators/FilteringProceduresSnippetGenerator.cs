using System.Collections.Generic;
using System.Linq;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FilteringProcedures)]
    public class FilteringProceduresSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public FilteringProceduresSnippetGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution()
            {
                SqlExpressionTranslator = new SqlServerExpressionTranslator(){ Configuration = construction.MeadowConfiguration },
                TypeNameMapper = new SqlDbTypeNameMapper()
            })
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            base.DeclareUnSupportedFeatures(declaration);
            declaration.NotSupportedRepetitionHandling();
            declaration.NotSupportedDbObjectNameOverriding();
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeViewName = GenerateKey();


        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldNameFullTree = GenerateKey();

        private readonly string _keyEntityParameters = GenerateKey();
        private readonly string _keyEntityParametersFullTree = GenerateKey();

        private readonly string _keyRemoveExisingProcedureName = GenerateKey();

        private readonly string _keyFilterIfNeededProcedureName = GenerateKey();
        private readonly string _keyFilterIfNeededProcedureNameFullTree = GenerateKey();

        private readonly string _keyReadChunkProcedureName = GenerateKey();
        private readonly string _keyReadChunkProcedureNameFullTree = GenerateKey();

        private readonly string _keyFilterResultsTable = GenerateKey();

        private readonly string _keyIndexEntityProcedureName = GenerateKey();
        private readonly string _keySearchIndexTableName = GenerateKey();
        private readonly string _keyIdFieldType = GenerateKey();

        private readonly string _keyCorpusFieldType = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);
            replacementList.Add(_keyFullTreeViewName, ProcessedType.NameConvention.FullTreeViewName);

            replacementList.Add(_keyIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name : "[NO-ID-FIELD]");
            replacementList.Add(_keyIdFieldNameFullTree,
                ProcessedType.HasId ? ProcessedType.IdParameterFullTree.Name : "[NO-ID-FIELD]");

            var entityParameters = ProcessedType.Parameters.Select(p => p.Name);

            replacementList.Add(_keyEntityParameters, string.Join(',', entityParameters));

            var entityParametersFullTree = ProcessedType.ParametersFullTree.Select(p => p.Name);

            replacementList.Add(_keyEntityParametersFullTree, string.Join(',', entityParametersFullTree));

            replacementList.Add(_keyRemoveExisingProcedureName,
                ProcessedType.NameConvention.RemoveExpiredFilterResultsProcedureName);


            replacementList.Add(_keyFilterIfNeededProcedureName,
                ProcessedType.NameConvention.PerformFilterIfNeededProcedureName);
            replacementList.Add(_keyFilterIfNeededProcedureNameFullTree,
                ProcessedType.NameConvention.PerformFilterIfNeededProcedureNameFullTree);
            replacementList.Add(_keyReadChunkProcedureName, ProcessedType.NameConvention.ReadChunkProcedureName);
            replacementList.Add(_keyReadChunkProcedureNameFullTree,
                ProcessedType.NameConvention.ReadChunkProcedureNameFullTree);

            replacementList.Add(_keyFilterResultsTable, ProcessedType.NameConvention.FilterResultsTableName);

            replacementList.Add(_keyIndexEntityProcedureName, ProcessedType.NameConvention.IndexEntityProcedureName);
            replacementList.Add(_keySearchIndexTableName, ProcessedType.NameConvention.SearchIndexTableName);
            replacementList.Add(_keyIdFieldType, ProcessedType.IdParameter.Type);

            replacementList.Add(_keyCorpusFieldType, ProcessedType.IndexCorpusParameter.Type);
            
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            
            var entityFilterSegment = entityFilterExpression.Success ? $" ({entityFilterExpression.Value}) AND " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyIndexEntityProcedureName}(@ResultId {_keyIdFieldType},@IndexCorpus {_keyCorpusFieldType}) AS
    
    IF EXISTS(SELECT 1 FROM {_keySearchIndexTableName} WHERE {_keySearchIndexTableName}.ResultId=@ResultId)
        BEGIN
            UPDATE {_keySearchIndexTableName} SET IndexCorpus=@IndexCorpus WHERE {_keySearchIndexTableName}.ResultId=@ResultId;
            
            SELECT TOP 1 * FROM {_keySearchIndexTableName} WHERE {_keySearchIndexTableName}.ResultId=@ResultId; 
        END
    ELSE
        BEGIN
            INSERT INTO {_keySearchIndexTableName} (ResultId,IndexCorpus) VALUES (@ResultId,@IndexCorpus)
            DECLARE @newId {_keyIdFieldType}=(IDENT_CURRENT('{_keySearchIndexTableName}'));
            SELECT * FROM {_keySearchIndexTableName} WHERE Id=@newId;
        END
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE {_keyRemoveExisingProcedureName}(@ExpirationTimeStamp BIGINT) AS
    DELETE FROM {_keyFilterResultsTable} WHERE {_keyFilterResultsTable}.ExpirationTimeStamp < @ExpirationTimeStamp
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterIfNeededProcedureName}(@SearchId NVARCHAR(32),
                                                  @ExpirationTimeStamp BIGINT,
                                                  @FilterExpression NVARCHAR(1024),
                                                  @SearchExpression NVARCHAR(1024),
                                                  @OrderExpression NVARCHAR(1024)) AS
    IF (SELECT Count(Id) from {_keyFilterResultsTable} where {_keyFilterResultsTable}.SearchId=@SearchId) = 0
    BEGIN
        SET @FilterExpression = coalesce(nullif(@FilterExpression, ''), '1=1')
        declare @query nvarchar(1600);
        declare @orderClause nvarchar(128);
        declare @resultCount bigint;
        SET @orderClause = '';

        IF NOT ISNULL(@OrderExpression,'')=''
            SET @orderClause = CONCAT(' ORDER BY ', @OrderExpression);

        IF ISNULL(@SearchExpression,'') = ''
            SET @query = CONCAT(
            'INSERT INTO {_keyFilterResultsTable} (SearchId,ResultId,ExpirationTimeStamp) ',
            'SELECT ''',@SearchId,''',{_keyTableName}.{_keyIdFieldName}, ',@ExpirationTimeStamp,' FROM {_keyTableName}  WHERE {_keyEntityFilterSegment} ' , @FilterExpression,@orderClause);
        ELSE
            SET @query = CONCAT(
            'INSERT INTO {_keyFilterResultsTable} (SearchId,ResultId,ExpirationTimeStamp) ',
            'SELECT ''',@SearchId,''',{_keyTableName}.{_keyIdFieldName}, ',@ExpirationTimeStamp,
            ' FROM {_keyTableName} INNER JOIN {_keySearchIndexTableName} ON {_keyTableName}.{_keyIdFieldName}={_keySearchIndexTableName}.ResultId WHERE {_keyEntityFilterSegment}(' ,
             @FilterExpression, ') AND (', @SearchExpression,')',@orderClause);

        execute sp_executesql @query
    END  
    SET @resultCount = (SELECT Count(SearchId) FROM {_keyFilterResultsTable} WHERE {_keyFilterResultsTable}.SearchId=@SearchId);
    SELECT @resultCount 'Count', @SearchId 'SearchId';
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterIfNeededProcedureNameFullTree}(@SearchId NVARCHAR(32),
                                                  @ExpirationTimeStamp BIGINT,
                                                  @FilterExpression NVARCHAR(1024),
                                                  @SearchExpression NVARCHAR(1024),
                                                  @OrderExpression NVARCHAR(1024)) AS
    IF (SELECT Count(Id) from {_keyFilterResultsTable} where {_keyFilterResultsTable}.SearchId=@SearchId) = 0
    BEGIN
        SET @FilterExpression = coalesce(nullif(@FilterExpression, ''), '1=1')
        declare @query nvarchar(1600);
        declare @orderClause nvarchar(1024);
        declare @groupExpression nvarchar(512);
        declare @resultCount bigint;
        SET @orderClause = '';
        SET @groupExpression = '';

        IF NOT ISNULL(@OrderExpression,'')=''
        BEGIN
            SET @groupExpression = REPLACE(@OrderExpression,' asc','');
            SET @groupExpression = REPLACE(@groupExpression,' desc','');
            SET @orderClause = CONCAT(' GROUP BY {_keyFullTreeViewName}.{_keyIdFieldNameFullTree},', @groupExpression ,' ORDER BY ', @OrderExpression);
        END
        ELSE
            SET @orderClause = CONCAT(' GROUP BY {_keyFullTreeViewName}.{_keyIdFieldNameFullTree}',' ORDER BY {_keyFullTreeViewName}.{_keyIdFieldNameFullTree}');
        
        IF ISNULL(@SearchExpression,'') = ''
            SET @query = CONCAT(
            'INSERT INTO {_keyFilterResultsTable} (SearchId,ResultId,ExpirationTimeStamp) ',
            'SELECT ''',@SearchId,''',{_keyFullTreeViewName}.{_keyIdFieldNameFullTree}, ',@ExpirationTimeStamp,' FROM {_keyFullTreeViewName} WHERE ' , @FilterExpression,@orderClause);
        ELSE
            SET @query = CONCAT(
            'INSERT INTO {_keyFilterResultsTable} (SearchId,ResultId,ExpirationTimeStamp) ',
            'SELECT ''',@SearchId,''',{_keyFullTreeViewName}.{_keyIdFieldNameFullTree}, ',@ExpirationTimeStamp,
            ' FROM {_keyFullTreeViewName} INNER JOIN {_keySearchIndexTableName} ON {_keyFullTreeViewName}.{_keyIdFieldNameFullTree}={_keySearchIndexTableName}.ResultId  WHERE (' ,
             @FilterExpression, ') AND (', @SearchExpression,')',@orderClause);
        execute sp_executesql @query
    END  
    SET @resultCount = (SELECT Count(SearchId) FROM {_keyFilterResultsTable} WHERE {_keyFilterResultsTable}.SearchId=@SearchId);
    SELECT @resultCount 'Count', @SearchId 'SearchId';
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadChunkProcedureName}(@Offset BIGINT,
                                    @Size BIGINT,
                                    @SearchId nvarchar(32)) AS
    SELECT {_keyTableName}.* FROM {_keyTableName} 
    INNER JOIN (SELECT * FROM {_keyFilterResultsTable} 
                               WHERE {_keyFilterResultsTable}.SearchId=@SearchId
                               ORDER BY {_keyFilterResultsTable}.Id ASC
                               OFFSET @Offset ROWS 
                               FETCH FIRST @Size ROWS ONLY) Fr 
        ON  {_keyTableName}.{_keyIdFieldName} = Fr.ResultId 
        order by Fr.Id 
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadChunkProcedureNameFullTree}(@Offset BIGINT,
                                    @Size BIGINT,
                                    @SearchId nvarchar(32)) AS
    SELECT {_keyFullTreeViewName}.* FROM {_keyFullTreeViewName} 
    INNER JOIN (SELECT * FROM {_keyFilterResultsTable} 
                               WHERE {_keyFilterResultsTable}.SearchId=@SearchId
                               ORDER BY {_keyFilterResultsTable}.Id ASC
                               OFFSET @Offset ROWS 
                               FETCH FIRST @Size ROWS ONLY) Fr 
        ON  {_keyFullTreeViewName}.{_keyIdFieldNameFullTree} = Fr.ResultId
        order by Fr.Id 
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}