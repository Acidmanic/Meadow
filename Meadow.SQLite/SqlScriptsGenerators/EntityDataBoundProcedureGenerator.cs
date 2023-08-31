using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
   

    public class EntityDataBoundProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        protected ProcessedType ProcessedType { get; }

        public EntityDataBoundProcedureGenerator(Type type, MeadowConfiguration configuration)
            : base(new SqLiteTypeNameMapper(), configuration)
        {
            if (RepetitionHandling != RepetitionHandling.Create)
            {
                LogUnSupportedRepetitionHandling("Data Bound Procedures");
            }

            ProcessedType = Process(type);
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyRangeProcedureName = GenerateKey();
        private readonly string _keyExistingValuesProcedureName = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyRangeProcedureName, ProcessedType.NameConvention.RangeProcedureName);
            replacementList.Add(_keyExistingValuesProcedureName, ProcessedType.NameConvention.ExistingValuesProcedureName);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyRangeProcedureName}(@FieldName TEXT) AS
    SELECT MAX(&@FieldName) 'Max', MIN(&@FieldName) 'Min' FROM {_keyTableName};
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyExistingValuesProcedureName}(@FieldName TEXT) AS
    SELECT DISTINCT &@FieldName 'Value' FROM {_keyTableName} ORDER BY &@FieldName ASC;
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}