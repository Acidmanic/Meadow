using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
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

        public FilteringProceduresGenerator(Type type) : base(new PostgreDbTypeNameMapper())
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

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName,ProcessedType.NameConvention.TableName);
            replacementList.Add(_keyDbQTableName,ProcessedType.NameConvention.TableName.DoubleQuot());
            replacementList.Add(_keyDbQFilterProcedureName, $"spPerform{ProcessedType.NameConvention.TableName}FilterIfNeeded".DoubleQuot());
            replacementList.Add(_keyDbQIdFieldName, ProcessedType.HasId?ProcessedType.IdParameter.Name.DoubleQuot():"[NO-ID-FIELD]");
            replacementList.Add(_keyDbQChunkProcedureName, $"spRead{ProcessedType.NameConvention.TableName}Chunk".DoubleQuot());
        }
        
        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function {"spRemoveExpiredFilterResults".DoubleQuot()}({"par_ExpirationTimeStamp".DoubleQuot()} BIGINT) 
    returns void as $$ 
begin
    delete from {"FilterResults".DoubleQuot()} where {"FilterResults".DoubleQuot()}.{"ExpirationTimeStamp".DoubleQuot()} >= {"par_ExpirationTimeStamp".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQFilterProcedureName} 
                ({"par_FilterHash".DoubleQuot()} TEXT,
                {"par_ExpirationTimeStamp".DoubleQuot()} BIGINT,
                {"par_FilterExpression".DoubleQuot()} TEXT) 
    returns setof {"FilterResults".DoubleQuot()} as $$
    declare sql text = '';
begin 
    if {"par_FilterExpression".DoubleQuot()} is null or {"par_FilterExpression".DoubleQuot()} ='' then
        {"par_FilterExpression".DoubleQuot()} = 'true';
    end if;
    sql = CONCAT('insert into {"FilterResults".DoubleQuot()} ({"FilterHash".DoubleQuot()}, {"ResultId".DoubleQuot()}, {"ExpirationTimeStamp".DoubleQuot()}) 
        select ''', {"par_FilterHash".DoubleQuot()},''',{_keyDbQTableName}.{_keyDbQIdFieldName}, ', {"par_ExpirationTimeStamp".DoubleQuot()},' from {_keyDbQTableName}
        where ',{"par_FilterExpression".DoubleQuot()},';');
    if not exists(select 1 from {"FilterResults".DoubleQuot()} where {"FilterHash".DoubleQuot()} = {"par_FilterHash".DoubleQuot()}) then
        execute sql; 
    end if;
    return query select * from {"FilterResults".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQChunkProcedureName}
                ({"par_Offset".DoubleQuot()} BIGINT,
                 {"par_Size".DoubleQuot()} BIGINT,
                 {"par_FilterHash".DoubleQuot()} TEXT) returns setof {_keyDbQTableName} as $$
begin
    return query select {_keyDbQTableName}.* from {_keyDbQTableName} 
        inner join (select * from {"FilterResults".DoubleQuot()} where {"FilterResults".DoubleQuot()}.{"FilterHash".DoubleQuot()}={"par_FilterHash".DoubleQuot()}) {"FR".DoubleQuot()}
        on {_keyDbQTableName}.{_keyDbQIdFieldName} = {"FR".DoubleQuot()}.{"ResultId".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}