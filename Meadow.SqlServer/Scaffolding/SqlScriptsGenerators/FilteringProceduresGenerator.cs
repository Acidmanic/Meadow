using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
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
            : base(new SqlDbTypeNameMapper(), configuration)
        {
            if (RepetitionHandling != RepetitionHandling.Create)
            {
                LogUnSupportedRepetitionHandling("FilteringProcedures");
            }

            ProcessedType = Process(type);
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyEntityParameters = GenerateKey();
        
        private readonly string _keyRemoveExisingProcedureName = GenerateKey();
        private readonly string _keyFilterIfNeededProcedureName = GenerateKey();
        private readonly string _keyReadChunkProcedureName = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name : "[NO-ID-FIELD]");

            var entityParameters = ProcessedType.Parameters.Select(p => p.Name);

            replacementList.Add(_keyEntityParameters, string.Join(',', entityParameters));
            
            replacementList.Add(_keyRemoveExisingProcedureName,ProcessedType.NameConvention.RemoveExpiredFilterResultsProcedureName);
            replacementList.Add(_keyFilterIfNeededProcedureName,ProcessedType.NameConvention.PerformFilterIfNeededProcedureName);
            replacementList.Add(_keyReadChunkProcedureName,ProcessedType.NameConvention.ReadChunkProcedureName);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE {_keyRemoveExisingProcedureName}(@ExpirationTimeStamp BIGINT) AS
    DELETE FROM FilterResults WHERE FilterResults.ExpirationTimeStamp < @ExpirationTimeStamp
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFilterIfNeededProcedureName}(@SearchId NVARCHAR(32),
                                                  @ExpirationTimeStamp BIGINT,
                                                  @FilterExpression NVARCHAR(1024)) AS
    IF (SELECT Count(Id) from FilterResults where FilterResults.SearchId=@SearchId) = 0
    BEGIN
        SET @FilterExpression = coalesce(nullif(@FilterExpression, ''), '1=1')
        declare @query nvarchar(1600) = CONCAT(
            'INSERT INTO FilterResults (SearchId,ResultId,ExpirationTimeStamp) ',
            'SELECT ''',@SearchId,''',{_keyIdFieldName}, ',@ExpirationTimeStamp,' FROM {_keyTableName} WHERE ' , @FilterExpression);
        execute sp_executesql @query
    END  
    SELECT FilterResults.* FROM FilterResults WHERE FilterResults.SearchId=@SearchId;
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
                  FROM {_keyTableName} INNER JOIN FilterResults on {_keyTableName}.{_keyIdFieldName} = FilterResults.ResultId
                  WHERE FilterResults.SearchId=@SearchId
              )
     SELECT {_keyEntityParameters}
     FROM Results_CTE
     WHERE RowNum >= (@Offset+1)
       AND RowNum < (@Offset+1) + @Size
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}