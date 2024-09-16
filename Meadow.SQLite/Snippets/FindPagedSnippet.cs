using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.FindPaged)]
public class FindPagedSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public string KeySelectColumns => Toolbox.GetSelectColumns(ColumnNameTranslation.DataOwnerDotColumnName);
    public string KeyTableName => Toolbox.ProcessedType.NameConvention.TableName;
    public string KeyFullTreeView => Toolbox.ProcessedType.NameConvention.FullTreeViewName;
    public string KeySearchIndexTableName => Toolbox.ProcessedType.NameConvention.SearchIndexTableName;
    public string KeyIdFieldName => Toolbox.IdFieldNameOrDefault("Id");
    public string KeyIdFieldNameFullTree => Toolbox.IdFieldNameOrDefaultFullTree("Id");
    public string KeyEntityFilterSegment => Toolbox.GetEntityFiltersWhereClause(" AND ", " ");

    public string IndexProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.IndexEntityProcedureName,
        pb => pb.Name("ResultId").Type(Toolbox.IdFieldTypeOrDefault())
            .Add().Name("IndexCorpus").Type<string>(),
        body, string.Empty,
        Toolbox.ProcessedType.NameConvention.TableName);

    public string FindPagedProcedure(string body) => FindPagedProcedure(body, Toolbox.ProcessedType.NameConvention.FindPagedProcedureName);
    public string FindPagedProcedureFullTree(string body) => FindPagedProcedure(body, Toolbox.ProcessedType.NameConvention.FindPagedProcedureNameFullTree);
    
    public string FindPagedProcedure(string body, string procedureName) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling, procedureName,
        pb => pb
            .Name("Offset").Type<int>().Add()
            .Name("Size").Type<int>().Add()
            .Name("FilterExpression").Type<string>().Add()
            .Name("SearchExpression").Type<string>().Add()
            .Name("OrderExpression").Type<string>(),
        body, string.Empty,
        Toolbox.ProcessedType.NameConvention.TableName);
    
    public string Template => @"
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
{IndexProcedure}
    UPDATE {KeySearchIndexTableName}  SET IndexCorpus=@IndexCorpus
        WHERE {KeySearchIndexTableName}.ResultId=@ResultId;
    INSERT INTO {KeySearchIndexTableName} (ResultId,IndexCorpus)
            SELECT @ResultId,@IndexCorpus WHERE NOT EXISTS(SELECT * FROM {KeySearchIndexTableName}
            WHERE {KeySearchIndexTableName}.ResultId=@ResultId);
    SELECT * FROM {KeySearchIndexTableName} WHERE ROWID=LAST_INSERT_ROWID();
    SELECT * FROM {KeySearchIndexTableName} WHERE {KeySearchIndexTableName}.ResultId=@ResultId OR ROWID = LAST_INSERT_ROWID() LIMIT 1;
{/IndexProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{FindPagedProcedure}
    SELECT {KeySelectColumns} FROM {KeyTableName}
    LEFT JOIN {KeySearchIndexTableName} ON {KeyTableName}.{KeyIdFieldName}={KeySearchIndexTableName}.ResultId
    WHERE (&@FilterExpression) AND (&@SearchExpression){KeyEntityFilterSegment}
    ORDER BY &@OrderExpression LIMIT @Offset,@Size;
{/FindPagedProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{FindPagedProcedureFullTree}
    SELECT {KeyFullTreeView}.* FROM {KeyFullTreeView} INNER JOIN 
    (SELECT DISTINCT {KeyIdFieldNameFullTree} 'Id' FROM {KeyFullTreeView}
    LEFT JOIN {KeySearchIndexTableName} ON {KeyFullTreeView}.{KeyIdFieldNameFullTree}={KeySearchIndexTableName}.ResultId
    WHERE (&@FilterExpression) AND (&@SearchExpression) ORDER BY &@OrderExpression LIMIT @Offset,@Size) Prx
    ON {KeyFullTreeView}.{KeyIdFieldNameFullTree}=Prx.Id;
{/FindPagedProcedureFullTree}
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}