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
        private readonly string _keyDbQFullTreeView = GenerateKey();

        private readonly string _keyDbQIdFieldName = GenerateKey();
        private readonly string _keyDbQIdFieldNameFullTree = GenerateKey();

        private readonly string _keyDbQFilterProcedureName = GenerateKey();
        private readonly string _keyDbQFilterProcedureNameFullTree = GenerateKey();

        private readonly string _keyDbQChunkProcedureName = GenerateKey();
        private readonly string _keyDbQChunkProcedureNameFullTree = GenerateKey();


        private readonly string _keyDbQRangeProcedureName = GenerateKey();
        private readonly string _keyDbQExistingValuesProcedureName = GenerateKey();


        private readonly string _keyDbQRemoveExpiredFilterResults = GenerateKey();
        private readonly string _keyDbQFilterResultsTableName = GenerateKey();

        protected static readonly string ll = "\"";

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyDbQTableName, ProcessedType.NameConvention.TableName.DoubleQuot());
            replacementList.Add(_keyDbQFullTreeView, ProcessedType.NameConvention.FullTreeViewName.DoubleQuot());

            replacementList.Add(_keyDbQFilterProcedureName,
                ProcessedType.NameConvention.PerformFilterIfNeededProcedureName.DoubleQuot());
            replacementList.Add(_keyDbQFilterProcedureNameFullTree,
                ProcessedType.NameConvention.PerformFilterIfNeededProcedureNameFullTree.DoubleQuot());

            replacementList.Add(_keyDbQIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name.DoubleQuot() : "[NO-ID-FIELD]");
            replacementList.Add(_keyDbQIdFieldNameFullTree,
                ProcessedType.HasId ? ProcessedType.IdParameterFullTree.Name.DoubleQuot() : "[NO-ID-FIELD]");

            replacementList.Add(_keyDbQChunkProcedureName,
                ProcessedType.NameConvention.ReadChunkProcedureName.DoubleQuot());
            replacementList.Add(_keyDbQChunkProcedureNameFullTree,
                ProcessedType.NameConvention.ReadChunkProcedureNameFullTree.DoubleQuot());

            replacementList.Add(_keyDbQRangeProcedureName,
                ProcessedType.NameConvention.RangeProcedureName.DoubleQuot());
            replacementList.Add(_keyDbQExistingValuesProcedureName,
                ProcessedType.NameConvention.ExistingValuesProcedureName.DoubleQuot());
            
            replacementList.Add(_keyDbQRemoveExpiredFilterResults,ProcessedType.NameConvention.RemoveExpiredFilterResultsProcedureName);
            replacementList.Add(_keyDbQFilterResultsTableName,ProcessedType.NameConvention.FilterResultsTableName.DoubleQuot());
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function {_keyDbQRemoveExpiredFilterResults}({"par_ExpirationTimeStamp".DoubleQuot()} BIGINT) 
    returns void as $$ 
begin
    delete from {_keyDbQFilterResultsTableName} where {_keyDbQFilterResultsTableName}.{"ExpirationTimeStamp".DoubleQuot()} < {"par_ExpirationTimeStamp".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQFilterProcedureName} 
                ({"par_SearchId".DoubleQuot()} TEXT,
                {"par_ExpirationTimeStamp".DoubleQuot()} BIGINT,
                {"par_FilterExpression".DoubleQuot()} TEXT) 
    returns setof {_keyDbQFilterResultsTableName} as $$
    declare sql text = '';
begin 
    if {"par_FilterExpression".DoubleQuot()} is null or {"par_FilterExpression".DoubleQuot()} ='' then
        {"par_FilterExpression".DoubleQuot()} = 'true';
    end if;
    sql = CONCAT('insert into {_keyDbQFilterResultsTableName} ({"SearchId".DoubleQuot()}, {"ResultId".DoubleQuot()}, {"ExpirationTimeStamp".DoubleQuot()}) 
        select ''', {"par_SearchId".DoubleQuot()},''',{_keyDbQTableName}.{_keyDbQIdFieldName}, ', {"par_ExpirationTimeStamp".DoubleQuot()},' from {_keyDbQTableName}
        where ',{"par_FilterExpression".DoubleQuot()},';');
    if not exists(select 1 from {_keyDbQFilterResultsTableName} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()}) then
        execute sql; 
    end if;
    return query select * from {_keyDbQFilterResultsTableName} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQFilterProcedureNameFullTree} 
                ({"par_SearchId".DoubleQuot()} TEXT,
                {"par_ExpirationTimeStamp".DoubleQuot()} BIGINT,
                {"par_FilterExpression".DoubleQuot()} TEXT) 
    returns setof {_keyDbQFilterResultsTableName} as $$
    declare sql text = '';
begin 
    if {"par_FilterExpression".DoubleQuot()} is null or {"par_FilterExpression".DoubleQuot()} ='' then
        {"par_FilterExpression".DoubleQuot()} = 'true';
    end if;
    sql = CONCAT('insert into {_keyDbQFilterResultsTableName} ({"SearchId".DoubleQuot()}, {"ResultId".DoubleQuot()}, {"ExpirationTimeStamp".DoubleQuot()}) 
        select distinct ''', {"par_SearchId".DoubleQuot()},''',{_keyDbQFullTreeView}.{_keyDbQIdFieldNameFullTree}, ', {"par_ExpirationTimeStamp".DoubleQuot()},' from {_keyDbQFullTreeView}
        where ',{"par_FilterExpression".DoubleQuot()},';');
    if not exists(select 1 from {_keyDbQFilterResultsTableName} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()}) then
        execute sql; 
    end if;
    return query select * from {_keyDbQFilterResultsTableName} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()};
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
        inner join (select * from {_keyDbQFilterResultsTableName} where {_keyDbQFilterResultsTableName}.{"SearchId".DoubleQuot()}={"par_SearchId".DoubleQuot()} LIMIT {"par_Size".DoubleQuot()} OFFSET {"par_Offset".DoubleQuot()}) {"FR".DoubleQuot()}
        on {_keyDbQTableName}.{_keyDbQIdFieldName} = {"FR".DoubleQuot()}.{"ResultId".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQChunkProcedureNameFullTree}
                ({"par_Offset".DoubleQuot()} BIGINT,
                 {"par_Size".DoubleQuot()} BIGINT,
                 {"par_SearchId".DoubleQuot()} TEXT) returns setof {_keyDbQFullTreeView} as $$
begin
    return query select {_keyDbQFullTreeView}.* from {_keyDbQFullTreeView} 
        inner join (select * from {_keyDbQFilterResultsTableName} where {_keyDbQFilterResultsTableName}.{"SearchId".DoubleQuot()}={"par_SearchId".DoubleQuot()} LIMIT {"par_Size".DoubleQuot()} OFFSET {"par_Offset".DoubleQuot()}) {"FR".DoubleQuot()}
        on {_keyDbQFullTreeView}.{_keyDbQIdFieldNameFullTree} = {"FR".DoubleQuot()}.{"ResultId".DoubleQuot()};
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}