using System.Collections.Generic;
using System.Linq;
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
            : base(new SqlDbTypeNameMapper(), construction, configurations)
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
            
            replacementList.Add(_keyIndexEntityProcedureName,ProcessedType.NameConvention.IndexEntityProcedureName);
            replacementList.Add(_keySearchIndexTableName,ProcessedType.NameConvention.SearchIndexTableName);
            replacementList.Add(_keyIdFieldType,ProcessedType.IdParameter.Type);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyIndexEntityProcedureName}(@ResultId {_keyIdFieldType},@IndexCorpus varchar(1024)) AS
    
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
                                                  @SearchExpression NVARCHAR(1024)) AS
    IF (SELECT Count(Id) from {_keyFilterResultsTable} where {_keyFilterResultsTable}.SearchId=@SearchId) = 0
    BEGIN
        SET @FilterExpression = coalesce(nullif(@FilterExpression, ''), '1=1')
        declare @query nvarchar(1600);

        IF ISNULL(@SearchExpression,'') = ''
            SET @query = CONCAT(
            'INSERT INTO {_keyFilterResultsTable} (SearchId,ResultId,ExpirationTimeStamp) ',
            'SELECT ''',@SearchId,''',{_keyTableName}.{_keyIdFieldName}, ',@ExpirationTimeStamp,' FROM {_keyTableName} WHERE ' , @FilterExpression);
        ELSE
            SET @query = CONCAT(
            'INSERT INTO {_keyFilterResultsTable} (SearchId,ResultId,ExpirationTimeStamp) ',
            'SELECT ''',@SearchId,''',{_keyTableName}.{_keyIdFieldName}, ',@ExpirationTimeStamp,
            ' FROM {_keyTableName} INNER JOIN {_keySearchIndexTableName} ON {_keyTableName}.{_keyIdFieldName}={_keySearchIndexTableName}.ResultId  WHERE (' ,
             @FilterExpression, ') AND (', @SearchExpression,')');

        execute sp_executesql @query
    END  
    SELECT {_keyFilterResultsTable}.* FROM {_keyFilterResultsTable} WHERE {_keyFilterResultsTable}.SearchId=@SearchId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterIfNeededProcedureNameFullTree}(@SearchId NVARCHAR(32),
                                                  @ExpirationTimeStamp BIGINT,
                                                  @FilterExpression NVARCHAR(1024),
                                                  @SearchExpression NVARCHAR(1024)) AS
    IF (SELECT Count(Id) from {_keyFilterResultsTable} where {_keyFilterResultsTable}.SearchId=@SearchId) = 0
    BEGIN
        SET @FilterExpression = coalesce(nullif(@FilterExpression, ''), '1=1')
        declare @query nvarchar(1600);

        IF ISNULL(@SearchExpression,'') = ''
            SET @query = CONCAT(
            'INSERT INTO {_keyFilterResultsTable} (SearchId,ResultId,ExpirationTimeStamp) ',
            'SELECT ''',@SearchId,''',{_keyFullTreeViewName}.{_keyIdFieldNameFullTree}, ',@ExpirationTimeStamp,' FROM {_keyFullTreeViewName} WHERE ' , @FilterExpression);
        ELSE
            SET @query = CONCAT(
            'INSERT INTO {_keyFilterResultsTable} (SearchId,ResultId,ExpirationTimeStamp) ',
            'SELECT ''',@SearchId,''',{_keyFullTreeViewName}.{_keyIdFieldNameFullTree}, ',@ExpirationTimeStamp,
            ' FROM {_keyFullTreeViewName} INNER JOIN {_keySearchIndexTableName} ON {_keyFullTreeViewName}.{_keyIdFieldNameFullTree}={_keySearchIndexTableName}.ResultId  WHERE (' ,
             @FilterExpression, ') AND (', @SearchExpression,')');
        
        execute sp_executesql @query
    END  
    SELECT {_keyFilterResultsTable}.* FROM {_keyFilterResultsTable} WHERE {_keyFilterResultsTable}.SearchId=@SearchId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadChunkProcedureName}(@Offset BIGINT,
                                    @Size BIGINT,
                                    @SearchId nvarchar(32)) AS
    ;WITH Results_CTE AS
              (
                  SELECT
                      {_keyTableName}.*,
                      ROW_NUMBER() OVER (ORDER BY {_keyTableName}.{_keyIdFieldName}) AS RowNum
                  FROM {_keyTableName} INNER JOIN {_keyFilterResultsTable} on {_keyTableName}.{_keyIdFieldName} = {_keyFilterResultsTable}.ResultId
                  WHERE {_keyFilterResultsTable}.SearchId=@SearchId
              )
     SELECT {_keyEntityParameters}
     FROM Results_CTE
     WHERE RowNum >= (@Offset+1)
       AND RowNum < (@Offset+1) + @Size
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
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}