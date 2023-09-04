using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class FilteringProceduresGenerator<TEntity> : FilteringProceduresGenerator
    {
        public FilteringProceduresGenerator(MeadowConfiguration configuration) : base(typeof(TEntity), configuration)
        {
        }
    }

    [CommonSnippet(CommonSnippets.FilteringProcedures)]
    public class FilteringProceduresGenerator : ByTemplateSqlGeneratorBase
    {
        protected ProcessedType ProcessedType { get; }

        public FilteringProceduresGenerator(Type type, MeadowConfiguration configuration)
            : base(new SqLiteTypeNameMapper(), configuration)
        {
            if (RepetitionHandling != RepetitionHandling.Create)
            {
                LogUnSupportedRepetitionHandling("FilteringProcedures");
            }

            ProcessedType = Process(type);
        }


        private readonly string _keyFilterProcedureName = GenerateKey();
        private readonly string _keyFilterProcedureNameFullTree = GenerateKey();
        private readonly string _keyReadChunkProcedureName = GenerateKey();
        private readonly string _keyReadChunkProcedureNameFullTree = GenerateKey();
        
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeView = GenerateKey();
        
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldNameFullTree = GenerateKey();
        
        private readonly string _keyRemoveExpiredFilterProcedure = GenerateKey();
        private readonly string _keyFilterResultsTableName = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyFilterProcedureName,ProcessedType.NameConvention.PerformFilterIfNeededProcedureName);
            replacementList.Add(_keyFilterProcedureNameFullTree,ProcessedType.NameConvention.PerformFilterIfNeededProcedureNameFullTree);
            
            replacementList.Add(_keyReadChunkProcedureName,ProcessedType.NameConvention.ReadChunkProcedureName);
            replacementList.Add(_keyReadChunkProcedureNameFullTree,ProcessedType.NameConvention.ReadChunkProcedureNameFullTree);
            
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);
            replacementList.Add(_keyFullTreeView, ProcessedType.NameConvention.FullTreeViewName);

            replacementList.Add(_keyIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name : "[NO-ID-FIELD]");
            replacementList.Add(_keyIdFieldNameFullTree,
                ProcessedType.HasId ? ProcessedType.IdParameterFullTree.Name : "[NO-ID-FIELD]");
            
            replacementList.Add(_keyRemoveExpiredFilterProcedure,
                ProcessedType.NameConvention.RemoveExpiredFilterResultsProcedureName);
            
            replacementList.Add(_keyFilterResultsTableName,ProcessedType.NameConvention.FilterResultsTableName);
            
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE {_keyRemoveExpiredFilterProcedure}(@ExpirationTimeStamp INTEGER)
AS
    DELETE FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.ExpirationTimeStamp < @ExpirationTimeStamp;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterProcedureName}(@SearchId TEXT,
                                                  @ExpirationTimeStamp INTEGER,
                                                  @FilterExpression TEXT)
AS
    INSERT INTO {_keyFilterResultsTableName} (SearchId, ResultId, ExpirationTimeStamp) 
    SELECT @SearchId,{_keyTableName}.{_keyIdFieldName},@ExpirationTimeStamp FROM {_keyTableName} WHERE &@FilterExpression 
    AND IIF((select count(Id) from {_keyFilterResultsTableName} where {_keyFilterResultsTableName}.SearchId=@SearchId)>0,false,true);

    SELECT {_keyFilterResultsTableName}.* FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.SearchId=@SearchId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterProcedureNameFullTree}(@SearchId TEXT,
                                                  @ExpirationTimeStamp INTEGER,
                                                  @FilterExpression TEXT)
AS
    INSERT INTO {_keyFilterResultsTableName} (SearchId, ResultId, ExpirationTimeStamp) 
    SELECT DISTINCT @SearchId,{_keyFullTreeView}.{_keyIdFieldNameFullTree},@ExpirationTimeStamp FROM {_keyFullTreeView} WHERE &@FilterExpression 
    AND IIF((select count(Id) from {_keyFilterResultsTableName} where {_keyFilterResultsTableName}.SearchId=@SearchId)>0,false,true);

    SELECT {_keyFilterResultsTableName}.* FROM {_keyFilterResultsTableName} WHERE {_keyFilterResultsTableName}.SearchId=@SearchId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadChunkProcedureName}(@Offset INTEGER,
                                      @Size INTEGER,
                                      @SearchId TEXT)
AS
    SELECT {_keyTableName}.* FROM {_keyTableName} INNER JOIN {_keyFilterResultsTableName} ON {_keyTableName}.{_keyIdFieldName} = {_keyFilterResultsTableName}.ResultId
    WHERE {_keyFilterResultsTableName}.SearchId=@SearchId LIMIT @Offset,@Size;  
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadChunkProcedureNameFullTree}(@Offset INTEGER,
                                      @Size INTEGER,
                                      @SearchId TEXT)
AS
    SELECT {_keyFullTreeView}.* FROM {_keyFullTreeView} INNER JOIN {_keyFilterResultsTableName} ON {_keyFullTreeView}.{_keyIdFieldNameFullTree} = {_keyFilterResultsTableName}.ResultId
    WHERE {_keyFilterResultsTableName}.SearchId=@SearchId LIMIT @Offset,@Size;  
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}