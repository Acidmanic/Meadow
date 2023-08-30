using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
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
            : base(new PostgreDbTypeNameMapper(), configuration)
        {
            if (RepetitionHandling != RepetitionHandling.Create)
            {
                LogUnSupportedRepetitionHandling("FilteringProcedures");
            }

            ProcessedType = Process(type);
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
            
            replacementList.Add(_keyDbQRangeProcedureName,ProcessedType.NameConvention.Range.DoubleQuot());
            replacementList.Add(_keyDbQExistingValuesProcedureName,ProcessedType.NameConvention.ExistingValues.DoubleQuot());
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function {"spRemoveExpiredFilterResults".DoubleQuot()}({"par_ExpirationTimeStamp".DoubleQuot()} BIGINT) 
    returns void as $$ 
begin
    delete from {"FilterResults".DoubleQuot()} where {"FilterResults".DoubleQuot()}.{"ExpirationTimeStamp".DoubleQuot()} < {"par_ExpirationTimeStamp".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQFilterProcedureName} 
                ({"par_SearchId".DoubleQuot()} TEXT,
                {"par_ExpirationTimeStamp".DoubleQuot()} BIGINT,
                {"par_FilterExpression".DoubleQuot()} TEXT) 
    returns setof {"FilterResults".DoubleQuot()} as $$
    declare sql text = '';
begin 
    if {"par_FilterExpression".DoubleQuot()} is null or {"par_FilterExpression".DoubleQuot()} ='' then
        {"par_FilterExpression".DoubleQuot()} = 'true';
    end if;
    sql = CONCAT('insert into {"FilterResults".DoubleQuot()} ({"SearchId".DoubleQuot()}, {"ResultId".DoubleQuot()}, {"ExpirationTimeStamp".DoubleQuot()}) 
        select ''', {"par_SearchId".DoubleQuot()},''',{_keyDbQTableName}.{_keyDbQIdFieldName}, ', {"par_ExpirationTimeStamp".DoubleQuot()},' from {_keyDbQTableName}
        where ',{"par_FilterExpression".DoubleQuot()},';');
    if not exists(select 1 from {"FilterResults".DoubleQuot()} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()}) then
        execute sql; 
    end if;
    return query select * from {"FilterResults".DoubleQuot()} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQChunkProcedureName}
                ({"par_Offset".DoubleQuot()} BIGINT,
                 {"par_Size".DoubleQuot()} BIGINT,
                 {"par_SearchId".DoubleQuot()} TEXT) returns setof {_keyDbQTableName} as $$
begin
    return query select {_keyDbQTableName}.* from {_keyDbQTableName} 
        inner join (select * from {"FilterResults".DoubleQuot()} where {"FilterResults".DoubleQuot()}.{"SearchId".DoubleQuot()}={"par_SearchId".DoubleQuot()} LIMIT {"par_Size".DoubleQuot()} OFFSET {"par_Offset".DoubleQuot()}) {"FR".DoubleQuot()}
        on {_keyDbQTableName}.{_keyDbQIdFieldName} = {"FR".DoubleQuot()}.{"ResultId".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQRangeProcedureName}({"par_FieldName".DoubleQuot()} TEXT)  
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
create function {_keyDbQExistingValuesProcedureName}({"par_FieldName".DoubleQuot()} TEXT)  
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