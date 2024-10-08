using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    public class EntityDataBoundProcedureSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public EntityDataBoundProcedureSnippetGenerator(Type type, MeadowConfiguration configuration)
            : base(new SnippetConstruction
                {
                    EntityType = type,
                    MeadowConfiguration = configuration
                }, SnippetConfigurations.Default(), new SnippetExecution
            {
                SqlTranslator = new PostgreSqlTranslator(configuration),
                TypeNameMapper = new PostgreDbTypeNameMapper()
            })
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            declaration.NotSupportedRepetitionHandling();
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyDbQTableName = GenerateKey();
        private readonly string _keyDbQIdFieldName = GenerateKey();
        private readonly string _keyDbQFilterProcedureName = GenerateKey();
        private readonly string _keyDbQChunkProcedureName = GenerateKey();
        private readonly string _keyDbQRangeProcedureName = GenerateKey();
        private readonly string _keyDbQExistingValuesProcedureName = GenerateKey();

        protected static readonly string ll = "\"";

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);
            replacementList.Add(_keyDbQTableName, ProcessedType.NameConvention.TableName.DoubleQuot());
            replacementList.Add(_keyDbQFilterProcedureName,
                $"spPerform{ProcessedType.NameConvention.TableName}FilterIfNeeded".DoubleQuot());
            replacementList.Add(_keyDbQIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name.DoubleQuot() : "[NO-ID-FIELD]");
            replacementList.Add(_keyDbQChunkProcedureName,
                $"spRead{ProcessedType.NameConvention.TableName}Chunk".DoubleQuot());

            replacementList.Add(_keyDbQRangeProcedureName,
                ProcessedType.NameConvention.RangeProcedureName.DoubleQuot());
            replacementList.Add(_keyDbQExistingValuesProcedureName,
                ProcessedType.NameConvention.ExistingValuesProcedureName.DoubleQuot());
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function {_keyDbQRangeProcedureName}({"par_FieldName".DoubleQuot()} TEXT)  
                returns table({"Max".DoubleQuot()} TEXT,{"Min".DoubleQuot()} TEXT) as $$
declare sql text = '';
begin
    sql = CONCAT('select TEXT(MAX({ll}',{"par_FieldName".DoubleQuot()},'{ll})) {"Max".DoubleQuot()}, TEXT(MIN({ll}' , {"par_FieldName".DoubleQuot()}, '{ll})) {"Min".DoubleQuot()} from {_keyDbQTableName};' );
     return QUERY execute sql ;    
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function {_keyDbQExistingValuesProcedureName}({"par_FieldName".DoubleQuot()} TEXT)  
                returns table({"Value".DoubleQuot()} TEXT) as $$
declare sql text = '';
begin
   
    sql = CONCAT('select distinct TEXT({ll}',{"par_FieldName".DoubleQuot()},'{ll}) {"Value".DoubleQuot()} from {_keyDbQTableName} order by {"Value".DoubleQuot()} asc');
     return QUERY execute sql ;    
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}