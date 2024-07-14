using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Meadow.Configuration;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class EntityDataBoundProcedureSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public EntityDataBoundProcedureSnippetGenerator(Type entityType, MeadowConfiguration configuration) :
            base(new SnippetConstruction
                {
                    EntityType = entityType,
                    MeadowConfiguration = configuration
                }, SnippetConfigurations.Default(),
                new SnippetExecution()
                {
                    SqlExpressionTranslator = new SqLiteExpressionTranslator(),
                    TypeNameMapper = new SqLiteTypeNameMapper()
                })
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            declaration.NotSupportedDbObjectNameOverriding();
            declaration.NotSupportedRepetitionHandling();
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyRangeProcedureName = GenerateKey();
        private readonly string _keyExistingValuesProcedureName = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, GetTableName());

            replacementList.Add(_keyRangeProcedureName, ProcessedType.NameConvention.RangeProcedureName);
            replacementList.Add(_keyExistingValuesProcedureName,
                ProcessedType.NameConvention.ExistingValuesProcedureName);
        }

        private string GetTableName()
        {
            return ProcessedType.NameConvention.TableName;
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE {_keyRangeProcedureName}(@FieldName TEXT) AS
    SELECT MAX(&@FieldName) 'Max', MIN(&@FieldName) 'Min' FROM {_keyTableName};
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE {_keyExistingValuesProcedureName}(@FieldName TEXT) AS
    SELECT DISTINCT &@FieldName 'Value' FROM {_keyTableName} ORDER BY &@FieldName ASC;
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}