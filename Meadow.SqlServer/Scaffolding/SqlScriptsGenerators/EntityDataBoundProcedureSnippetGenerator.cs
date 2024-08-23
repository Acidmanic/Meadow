using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    public class EntityDataBoundProcedureSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {

        public EntityDataBoundProcedureSnippetGenerator(Type type, MeadowConfiguration configuration)
            : base(new SnippetConstruction
            {
                EntityType = type,
                MeadowConfiguration = configuration
            },SnippetConfigurations.Default(),
                new SnippetExecution()
                {
                    SqlTranslator = new SqlServerTranslator(configuration ),
                    TypeNameMapper = new SqlDbTypeNameMapper()
                })
        { }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyRangeProcedureName = GenerateKey();
        private readonly string _keyExistingValuesProcedureName = GenerateKey();


        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            base.DeclareUnSupportedFeatures(declaration);
            
            declaration.NotSupportedRepetitionHandling();
            declaration.NotSupportedDbObjectNameOverriding();
        }

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