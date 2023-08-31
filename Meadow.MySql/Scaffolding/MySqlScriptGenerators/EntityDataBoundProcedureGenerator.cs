using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class EntityDataBoundProcedureGenerator:ByTemplateSqlGeneratorBase
    {

        private readonly string _keyRangeProcedureName = GenerateKey();
        private readonly string _keyExistingProcedureName = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private  ProcessedType ProcessedType { get; }
        
        public EntityDataBoundProcedureGenerator(Type type, MeadowConfiguration configuration) 
            : base(new MySqlDbTypeNameMapper(), configuration)
        {
            ProcessedType = Process(type);
            if (RepetitionHandling != RepetitionHandling.Create)
            {
                LogUnSupportedRepetitionHandling("Data Bound");
            }
            
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyRangeProcedureName,ProcessedType.NameConvention.RangeProcedureName);
            replacementList.Add(_keyExistingProcedureName,ProcessedType.NameConvention.ExistingValuesProcedureName);
            replacementList.Add(_keyTableName,ProcessedType.NameConvention.TableName);
        }

        protected override string Template => $@"
DROP PROCEDURE IF EXISTS {_keyRangeProcedureName};
CREATE PROCEDURE {_keyRangeProcedureName}(IN FieldName nvarchar(32))
BEGIN
    set @query = CONCAT('SELECT MAX(',FieldName,') \'Max\', MIN(',FieldName,') \'Min\' FROM {_keyTableName};' );
    PREPARE stmt FROM @query;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt; 
END;
-- ---------------------------------------------------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS {_keyExistingProcedureName};
CREATE PROCEDURE {_keyExistingProcedureName}(IN FieldName nvarchar(32))
BEGIN
    set @query = CONCAT('SELECT DISTINCT ',FieldName,' \'Value\' FROM {_keyTableName} ORDER BY ',FieldName,' ASC');
    PREPARE stmt FROM @query;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt; 
END;
-- ---------------------------------------------------------------------------------------------------------------------".Trim();
    }
}