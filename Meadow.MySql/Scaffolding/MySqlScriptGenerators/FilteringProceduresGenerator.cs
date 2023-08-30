using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class FilteringProceduresGenerator<TEntity> : FilteringProceduresGenerator
    {
        public FilteringProceduresGenerator(MeadowConfiguration configuration)
            : base(typeof(TEntity), configuration)
        {
        }
    }

    [CommonSnippet(CommonSnippets.FilteringProcedures)]
    public class FilteringProceduresGenerator : ByTemplateSqlGeneratorBase
    {
        protected ProcessedType ProcessedType { get; private set; }

        protected Type EntityType { get; }

        public FilteringProceduresGenerator(Type type, MeadowConfiguration configuration)
            : base(new MySqlDbTypeNameMapper(), configuration)
        {
            if (RepetitionHandling != RepetitionHandling.Create)
            {
                LogUnSupportedRepetitionHandling("FilteringProcedures");
            }

            EntityType = type;
            ProcessedType = Process(EntityType);
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name : "[NO-ID-FIELD]");
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS spRemoveExpiredFilterResults;
CREATE PROCEDURE spRemoveExpiredFilterResults(IN ExpirationTimeStamp bigint(16))
BEGIN
    DELETE FROM FilterResults WHERE FilterResults.ExpirationTimeStamp < ExpirationTimeStamp;
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spPerform{_keyTableName}FilterIfNeeded(
                                                  IN SearchId nvarchar(32),
                                                  IN ExpirationTimeStamp bigint(16),
                                                  IN FilterExpression nvarchar(1024))
BEGIN
    if not exists(select 1 from FilterResults where FilterResults.SearchId=SearchId) then
        IF FilterExpression IS NULL OR FilterExpression = '' THEN
            set FilterExpression = 'TRUE';
        END IF;
        set @query = CONCAT(
            'insert into FilterResults (SearchId,ResultId,ExpirationTimeStamp)',
            'select \'',SearchId,'\',{_keyTableName}.{_keyIdFieldName},',ExpirationTimeStamp,
            ' from {_keyTableName} WHERE ' , FilterExpression,';');
        PREPARE stmt FROM @query;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt; 
    end if;
    SELECT FilterResults.* FROM FilterResults WHERE FilterResults.SearchId=SearchId;
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spRead{_keyTableName}Chunk(IN Offset bigint(16),
                                      IN Size bigint(16),
                                      IN SearchId nvarchar(32))
BEGIN
    select {_keyTableName}.* from {_keyTableName} inner join FilterResults on {_keyTableName}.{_keyIdFieldName} = FilterResults.ResultId
    where FilterResults.SearchId=SearchId limit offset,size;  
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE sp{_keyTableName}Range(IN FieldName nvarchar(32))
BEGIN
    set @query = CONCAT('SELECT MAX(',FieldName,') \'Max\', MIN(',FieldName,') \'Min\' FROM {_keyTableName};' );
    PREPARE stmt FROM @query;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt; 
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE sp{_keyTableName}ExistingValues(IN FieldName nvarchar(32))
BEGIN
    set @query = CONCAT('SELECT DISTINCT ',FieldName,' \'Value\' FROM {_keyTableName} ORDER BY ',FieldName,' ASC');
    PREPARE stmt FROM @query;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt; 
END;
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}