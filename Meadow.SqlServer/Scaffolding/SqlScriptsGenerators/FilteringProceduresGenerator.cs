using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class FilteringProceduresGenerator<TEntity> : FilteringProceduresGenerator
    {
        public FilteringProceduresGenerator() : base(typeof(TEntity))
        {
        }
    }
    
    [CommonSnippet(CommonSnippets.FilteringProcedures)]
    public class FilteringProceduresGenerator:ByTemplateSqlGeneratorBase
    {
        protected ProcessedType ProcessedType { get; }

        public FilteringProceduresGenerator(Type type) : base(new SqlDbTypeNameMapper())
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

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName,ProcessedType.NameConvention.TableName);
            
            replacementList.Add(_keyIdFieldName, ProcessedType.HasId?ProcessedType.IdParameter.Name:"[NO-ID-FIELD]");

            var entityParameters = ProcessedType.Parameters.Select(p => p.Name);
            
            replacementList.Add(_keyEntityParameters,string.Join(',',entityParameters));
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spRemoveExpiredFilterResults(@ExpirationTimeStamp BIGINT) AS
    DELETE FROM FilterResults WHERE FilterResults.ExpirationTimeStamp >= @ExpirationTimeStamp
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spPerform{_keyTableName}FilterIfNeeded(@FilterHash NVARCHAR(128),
                                                  @ExpirationTimeStamp BIGINT,
                                                  @WhereClause NVARCHAR(1024)) AS
BEGIN
    IF (SELECT Count(Id) from FilterResults where FilterResults.FilterHash=@FilterHash) = 0

        declare @query nvarchar(1600) = CONCAT(
            'INSERT INTO FilterResults (FilterHash,ResultId,ExpirationTimeStamp)',CHAR(13),
            'SELECT ''',@FilterHash,''',{_keyIdFieldName}, ',@ExpirationTimeStamp,' FROM {_keyTableName}' , @WhereClause,CHAR(13),'GO');
        execute sp_executesql @query
    END  
    SELECT FilterResults.* FROM FilterResults WHERE FilterResults.FilterHash=FilterHash;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spRead{_keyTableName}Chunk(@Offset BIGINT,
                                    @Size BIGINT,
                                    @FilterHash nvarchar(128)) AS
    ;WITH Results_CTE AS
              (
                  SELECT
                      {_keyTableName}.*,
                      ROW_NUMBER() OVER (ORDER BY {_keyTableName}.{_keyIdFieldName}) AS RowNum
                  FROM {_keyTableName} INNER JOIN FilterResults on {_keyTableName}.{_keyIdFieldName} = FilterResults.ResultId
                  WHERE FilterResults.FilterHash=@FilterHash
              )
     SELECT {_keyEntityParameters}
     FROM Results_CTE
     WHERE RowNum >= @Offset
       AND RowNum < @Offset + @Size
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}