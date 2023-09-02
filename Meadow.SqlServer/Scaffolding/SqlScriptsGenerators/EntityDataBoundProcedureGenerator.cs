using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class EntityDataBoundProcedureGenerator : ByTemplateSqlGeneratorBase
    {
        protected ProcessedType ProcessedType { get; }

        public EntityDataBoundProcedureGenerator(Type type, MeadowConfiguration configuration)
            : base(new SqlDbTypeNameMapper(), configuration)
        {
            if (RepetitionHandling != RepetitionHandling.Create)
            {
                LogUnSupportedRepetitionHandling("Data Bound");
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
            replacementList.Add(_keyExistingValuesProcedureName,
                ProcessedType.NameConvention.ExistingValuesProcedureName);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE {_keyRangeProcedureName}(@FieldName nvarchar(32)) AS

    declare @query nvarchar(1024) = CONCAT('SELECT MAX(',@FieldName,') ''Max'', MIN(',@FieldName,') ''Min'' FROM {_keyTableName}' );
    execute sp_executesql @query
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE {_keyExistingValuesProcedureName}(@FieldName nvarchar(32)) AS

    declare @query nvarchar(1024) = CONCAT('SELECT DISTINCT ',@FieldName,' ''Value'' FROM {_keyTableName} ORDER BY ',@FieldName,' ASC');
    execute sp_executesql @query
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}