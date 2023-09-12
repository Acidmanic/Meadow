using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.FilteringProcedures)]
    public class FilteringProceduresSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public FilteringProceduresSnippetGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(new PostgreDbTypeNameMapper(), construction, configurations)
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            base.DeclareUnSupportedFeatures(declaration);

            declaration.NotSupportedRepetitionHandling();
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
        
        private readonly string _keyDbQIndexProcedureName = GenerateKey();
        private readonly string _keyDbQSearchIndexTableName = GenerateKey();
        private readonly string _keyIdTypeName = GenerateKey();

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

            replacementList.Add(_keyDbQRemoveExpiredFilterResults,
                ProcessedType.NameConvention.RemoveExpiredFilterResultsProcedureName);
            replacementList.Add(_keyDbQFilterResultsTableName,
                ProcessedType.NameConvention.FilterResultsTableName.DoubleQuot());
            
            replacementList.Add(_keyIdTypeName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Type : "[NO-ID-FIELD]");

            replacementList.Add(_keyDbQIndexProcedureName, ProcessedType.NameConvention.IndexEntityProcedureName.DoubleQuot());
            replacementList.Add(_keyDbQSearchIndexTableName, ProcessedType.NameConvention.SearchIndexTableName.DoubleQuot());
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQIndexProcedureName}({"par_ResultId".DoubleQuot()} {_keyIdTypeName},{"par_IndexCorpus".DoubleQuot()} TEXT) returns setof {_keyDbQSearchIndexTableName} as $$
    declare updateCount int := 0;
    begin
        updateCount := (select count(*) from {_keyDbQSearchIndexTableName} where {_keyDbQSearchIndexTableName}.{"ResultId".DoubleQuot()}={"par_ResultId".DoubleQuot()});
        if (updateCount > 0) then
            return query
                    update {_keyDbQSearchIndexTableName} set 
                    {"IndexCorpus".DoubleQuot()}={"par_IndexCorpus".DoubleQuot()}
                    where {_keyDbQSearchIndexTableName}.{"ResultId".DoubleQuot()}={"par_ResultId".DoubleQuot()}
                    returning *;
        else
            return query
                insert into {_keyDbQSearchIndexTableName} ({"ResultId".DoubleQuot()},{"IndexCorpus".DoubleQuot()})
                values ({"par_ResultId".DoubleQuot()},{"par_IndexCorpus".DoubleQuot()})
            returning * ;
        end if;
    end;
    $$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQRemoveExpiredFilterResults}({"par_ExpirationTimeStamp".DoubleQuot()} BIGINT) 
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
                {"par_FilterExpression".DoubleQuot()} TEXT,
                {"par_SearchExpression".DoubleQuot()} TEXT, 
                {"par_OrderExpression".DoubleQuot()} TEXT) 
    returns setof {_keyDbQFilterResultsTableName} as $$
    declare sql text = '';
    declare orderClause text = '';
begin 
    if {"par_FilterExpression".DoubleQuot()} is null or {"par_FilterExpression".DoubleQuot()} ='' then
        {"par_FilterExpression".DoubleQuot()} = 'true';
    end if;
    if {"par_OrderExpression".DoubleQuot()} is not null and not {"par_OrderExpression".DoubleQuot()} = '' then
        orderClause = CONCAT(' order by ' , {"par_OrderExpression".DoubleQuot()},' ');
    end if;
    if {"par_SearchExpression".DoubleQuot()} is null or {"par_SearchExpression".DoubleQuot()} ='' then
        sql = CONCAT('insert into {_keyDbQFilterResultsTableName} ({"SearchId".DoubleQuot()}, {"ResultId".DoubleQuot()}, {"ExpirationTimeStamp".DoubleQuot()}) 
            select ''', {"par_SearchId".DoubleQuot()},''',{_keyDbQTableName}.{_keyDbQIdFieldName}, ', {"par_ExpirationTimeStamp".DoubleQuot()},' from {_keyDbQTableName}
            where ',{"par_FilterExpression".DoubleQuot()}, orderClause,';');
    else
        sql = CONCAT('insert into {_keyDbQFilterResultsTableName} ({"SearchId".DoubleQuot()}, {"ResultId".DoubleQuot()}, {"ExpirationTimeStamp".DoubleQuot()}) 
            select ''', {"par_SearchId".DoubleQuot()},''',{_keyDbQTableName}.{_keyDbQIdFieldName}, ', {"par_ExpirationTimeStamp".DoubleQuot()},' from {_keyDbQTableName}
            inner join {_keyDbQSearchIndexTableName} on {_keyDbQTableName}.{_keyDbQIdFieldName}={_keyDbQSearchIndexTableName}.{"ResultId".DoubleQuot()}
            where (',{"par_FilterExpression".DoubleQuot()},') AND (',{"par_SearchExpression".DoubleQuot()},')',orderClause,';');
    end if;
    if not exists(select 1 from {_keyDbQFilterResultsTableName} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()}) then
        execute sql; 
    end if;
    return query select * from {_keyDbQFilterResultsTableName} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()} order by {"Id".DoubleQuot()} ASC;
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function {_keyDbQFilterProcedureNameFullTree} 
                ({"par_SearchId".DoubleQuot()} TEXT,
                {"par_ExpirationTimeStamp".DoubleQuot()} BIGINT,
                {"par_FilterExpression".DoubleQuot()} TEXT,
                {"par_SearchExpression".DoubleQuot()} TEXT, 
                {"par_OrderExpression".DoubleQuot()} TEXT)  
    returns setof {_keyDbQFilterResultsTableName} as $$
    declare sql text = '';
    declare orderClause text = '';
    declare groupByExpression text = '';
begin 
    if {"par_FilterExpression".DoubleQuot()} is null or {"par_FilterExpression".DoubleQuot()} ='' then
        {"par_FilterExpression".DoubleQuot()} = 'true';
    end if;
    if {"par_OrderExpression".DoubleQuot()} is not null and not {"par_OrderExpression".DoubleQuot()} = '' then
        groupByExpression =  REGEXP_REPLACE(REGEXP_REPLACE({"par_OrderExpression".DoubleQuot()},'\s+asc','','i'),'\s+desc','','i');
        orderClause = CONCAT(' group by {_keyDbQIdFieldNameFullTree}, ', groupByExpression , ' order by ' , {"par_OrderExpression".DoubleQuot()},' ');
    end if;
    if {"par_SearchExpression".DoubleQuot()} is null or {"par_SearchExpression".DoubleQuot()} ='' then
        sql = CONCAT('insert into {_keyDbQFilterResultsTableName} ({"SearchId".DoubleQuot()}, {"ResultId".DoubleQuot()}, {"ExpirationTimeStamp".DoubleQuot()}) 
            select ''', {"par_SearchId".DoubleQuot()},''',{_keyDbQFullTreeView}.{_keyDbQIdFieldNameFullTree}, ', {"par_ExpirationTimeStamp".DoubleQuot()},' 
            from {_keyDbQFullTreeView} where ',{"par_FilterExpression".DoubleQuot()},orderClause,';');
    else
        sql = CONCAT('insert into {_keyDbQFilterResultsTableName} ({"SearchId".DoubleQuot()}, {"ResultId".DoubleQuot()}, {"ExpirationTimeStamp".DoubleQuot()}) 
            select ''', {"par_SearchId".DoubleQuot()},''',{_keyDbQFullTreeView}.{_keyDbQIdFieldNameFullTree}, ', {"par_ExpirationTimeStamp".DoubleQuot()},' from {_keyDbQFullTreeView}
            inner join {_keyDbQSearchIndexTableName} on {_keyDbQFullTreeView}.{_keyDbQIdFieldNameFullTree}={_keyDbQSearchIndexTableName}.{"ResultId".DoubleQuot()}
            where (',{"par_FilterExpression".DoubleQuot()},') AND (',{"par_SearchExpression".DoubleQuot()},')',orderClause,';');
    end if;
    if not exists(select 1 from {_keyDbQFilterResultsTableName} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()}) then
        execute sql; 
    end if;
    return query select * from {_keyDbQFilterResultsTableName} where {"SearchId".DoubleQuot()} = {"par_SearchId".DoubleQuot()}  order by {"Id".DoubleQuot()} ASC;
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
        on {_keyDbQTableName}.{_keyDbQIdFieldName} = {"FR".DoubleQuot()}.{"ResultId".DoubleQuot()} order by {"FR".DoubleQuot()}.{"Id".DoubleQuot()} ASC;
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
        on {_keyDbQFullTreeView}.{_keyDbQIdFieldNameFullTree} = {"FR".DoubleQuot()}.{"ResultId".DoubleQuot()}  order by {"FR".DoubleQuot()}.{"Id".DoubleQuot()} ASC;
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}