using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
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

        public FilteringProceduresGenerator(Type type) : base(new MySqlDbTypeNameMapper())
        {
            ProcessedType = Process(type);
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName,ProcessedType.NameConvention.TableName);
            
            replacementList.Add(_keyIdFieldName, ProcessedType.HasId?ProcessedType.IdParameter.Name:"[NO-ID-FIELD]");
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS spRemoveExpiredFilterResults;
CREATE PROCEDURE spRemoveExpiredFilterResults(IN ExpirationTimeStamp bigint(16))
BEGIN
    DELETE FROM FilterResults WHERE FilterResults.ExpirationTimeStamp >= ExpirationTimeStamp;
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spPerform{_keyTableName}FilterIfNeeded(IN FilterHash nvarchar(128),
                                                  IN ExpirationTimeStamp bigint(16),
                                                  IN WhereClause nvarchar(1024))
BEGIN
    if not exists(select 1 from FilterResults where FilterResults.FilterHash=FilterHash) then
        set @query = CONCAT(
            'insert into FilterResults (FilterHash,ResultId,ExpirationTimeStamp)',
            'select \'',FilterHash,'\',{_keyTableName}.{_keyIdFieldName},',ExpirationTimeStamp,' from {_keyTableName} ' , WhereClause,';');
        PREPARE stmt FROM @query;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
            
    end if;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spRead{_keyTableName}Chunk(IN Offset bigint(16),
                                      IN Size bigint(16),
                                      IN FilterHash nvarchar(128))
BEGIN
    select {_keyTableName}.* from {_keyTableName} inner join FilterResults on {_keyTableName}.{_keyIdFieldName} = FilterResults.ResultId
    where FilterResults.FilterHash=FilterHash limit offset,size;  
END;
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}