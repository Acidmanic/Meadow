using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.FilteringProcedures)]
    public class FilteringProceduresSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public FilteringProceduresSnippetGenerator(
            SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(new MySqlDbTypeNameMapper(), construction, configurations)
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            declaration.NotSupportedRepetitionHandling();
            declaration.NotSupportedDbObjectNameOverriding();
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyRemoveExistingProcedureName = GenerateKey();

        private readonly string _keyFilterIfNeededProcedureName = GenerateKey();
        private readonly string _keyReadChunkProcedureName = GenerateKey();

        private readonly string _keyFilterIfNeededProcedureNameFullTree = GenerateKey();
        private readonly string _keyReadChunkProcedureNameFullTree = GenerateKey();

        private readonly string _keyFullTreeViewName = GenerateKey();
        private readonly string _keyIdFieldNameFullTree = GenerateKey();

        private readonly string _keyFilterResultsTableName = GenerateKey();
        private readonly string _keyIndexProcedureName = GenerateKey();
        
        private readonly string _keyIdTypeName = GenerateKey();
        private readonly string _keySearchIndexTableName = GenerateKey();
        
        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name : "[NO-ID-FIELD]");

            replacementList.Add(_keyIdTypeName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Type : "[NO-ID-FIELD]");
            
            

            replacementList.Add(_keyRemoveExistingProcedureName,
                ProcessedType.NameConvention.RemoveExpiredFilterResultsProcedureName);

            replacementList.Add(_keyFilterIfNeededProcedureName,
                ProcessedType.NameConvention.PerformFilterIfNeededProcedureName);
            replacementList.Add(_keyFilterIfNeededProcedureNameFullTree,
                ProcessedType.NameConvention.PerformFilterIfNeededProcedureNameFullTree);

            replacementList.Add(_keyReadChunkProcedureName, ProcessedType.NameConvention.ReadChunkProcedureName);
            replacementList.Add(_keyReadChunkProcedureNameFullTree,
                ProcessedType.NameConvention.ReadChunkProcedureNameFullTree);

            replacementList.Add(_keyFullTreeViewName, ProcessedType.NameConvention.FullTreeViewName);
            replacementList.Add(_keyIdFieldNameFullTree, ProcessedType.IdParameterFullTree.Name);

            replacementList.Add(_keyFilterResultsTableName, ProcessedType.NameConvention.FilterResultsTableName);
            
            replacementList.Add(_keyIndexProcedureName, ProcessedType.NameConvention.IndexEntityProcedureName);
            replacementList.Add(_keySearchIndexTableName, ProcessedType.NameConvention.SearchIndexTableName);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyIndexProcedureName}(IN ResultId {_keyIdTypeName},IN IndexCorpus varchar(1024))
BEGIN
    IF EXISTS(SELECT 1 FROM {_keySearchIndexTableName} WHERE {_keySearchIndexTableName}.ResultId=ResultId) THEN
        UPDATE {_keySearchIndexTableName} SET {_keySearchIndexTableName}.IndexCorpus=IndexCorpus;
        SELECT * FROM {_keySearchIndexTableName} WHERE {_keySearchIndexTableName}.ResultId=ResultId;
    ELSE
        INSERT INTO {_keySearchIndexTableName} (ResultId,IndexCorpus) VALUES (ResultId,IndexCorpus);
        SET @nid = (select LAST_INSERT_ID());
        SELECT * FROM {_keySearchIndexTableName} WHERE {_keySearchIndexTableName}.Id=@nid;
    END IF;
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyRemoveExistingProcedureName}(IN ExpirationTimeStamp bigint(16))
BEGIN
    DELETE FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.ExpirationTimeStamp < ExpirationTimeStamp;
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterIfNeededProcedureName}(
                                                  IN SearchId nvarchar(32),
                                                  IN ExpirationTimeStamp bigint(16),
                                                  IN FilterExpression nvarchar(1024),
                                                  IN SearchExpression nvarchar(1024))
BEGIN
    if not exists(select 1 from {_keyFilterResultsTableName} where {_keyFilterResultsTableName}.SearchId=SearchId) then
        IF FilterExpression IS NULL OR FilterExpression = '' THEN
            set FilterExpression = 'TRUE';
        END IF;
        set @query ='';
        IF SearchExpression IS NULL OR SearchExpression = '' THEN
            set @query = CONCAT(
            'insert into {_keyFilterResultsTableName} (SearchId,ResultId,ExpirationTimeStamp)',
            'select \'',SearchId,'\',{_keyTableName}.{_keyIdFieldName},',ExpirationTimeStamp,
            ' from {_keyTableName}  WHERE ' , FilterExpression, ';');
        ELSE
            set @query = CONCAT(
                'insert into {_keyFilterResultsTableName} (SearchId,ResultId,ExpirationTimeStamp)',
                'select \'',SearchId,'\',{_keyTableName}.{_keyIdFieldName},',ExpirationTimeStamp,
                ' from {_keyTableName} inner join {_keySearchIndexTableName} on {_keyTableName}.{_keyIdFieldName}={_keySearchIndexTableName}.ResultId WHERE (' , FilterExpression, ') AND (', SearchExpression, ');');
        END IF;
        
        PREPARE stmt FROM @query;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt; 
    end if;
    SELECT {_keyFilterResultsTableName}.* FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.SearchId=SearchId;
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterIfNeededProcedureNameFullTree}(
                                                  IN SearchId nvarchar(32),
                                                  IN ExpirationTimeStamp bigint(16),
                                                  IN FilterExpression nvarchar(1024),
                                                  IN SearchExpression nvarchar(1024))
BEGIN
    if not exists(select 1 from {_keyFilterResultsTableName} where {_keyFilterResultsTableName}.SearchId=SearchId) then
        IF FilterExpression IS NULL OR FilterExpression = '' THEN
            set FilterExpression = 'TRUE';
        END IF;
        set @query ='';
        IF SearchExpression IS NULL OR SearchExpression = '' THEN
            set @query = CONCAT(
            'insert into {_keyFilterResultsTableName} (SearchId,ResultId,ExpirationTimeStamp)',
            'select \'',SearchId,'\',{_keyFullTreeViewName}.{_keyIdFieldNameFullTree},',ExpirationTimeStamp,
            ' from {_keyFullTreeViewName}  WHERE ' , FilterExpression, ';');
        ELSE
            set @query = CONCAT(
                'insert into {_keyFilterResultsTableName} (SearchId,ResultId,ExpirationTimeStamp)',
                'select \'',SearchId,'\',{_keyFullTreeViewName}.{_keyIdFieldNameFullTree},',ExpirationTimeStamp,
                ' from {_keyFullTreeViewName} inner join {_keySearchIndexTableName} on {_keyFullTreeViewName}.{_keyIdFieldNameFullTree}={_keySearchIndexTableName}.ResultId WHERE (' , FilterExpression, ') AND (', SearchExpression, ');');
        END IF;
        
        PREPARE stmt FROM @query;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt; 
    end if;
    SELECT {_keyFilterResultsTableName}.* FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.SearchId=SearchId;
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadChunkProcedureName}(IN Offset bigint(16),
                                      IN Size bigint(16),
                                      IN SearchId nvarchar(32))
BEGIN
    select {_keyTableName}.* from {_keyTableName} inner join {_keyFilterResultsTableName} on {_keyTableName}.{_keyIdFieldName} = {_keyFilterResultsTableName}.ResultId
    where {_keyFilterResultsTableName}.SearchId=SearchId limit offset,size;  
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadChunkProcedureNameFullTree}(IN Offset bigint(16),
                                      IN Size bigint(16),
                                      IN SearchId nvarchar(32))
BEGIN
    select {_keyFullTreeViewName}.* from {_keyFullTreeViewName} inner join {_keyFilterResultsTableName} on {_keyFullTreeViewName}.{_keyIdFieldNameFullTree} = {_keyFilterResultsTableName}.ResultId
    where {_keyFilterResultsTableName}.SearchId=SearchId limit offset,size;  
END;
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}