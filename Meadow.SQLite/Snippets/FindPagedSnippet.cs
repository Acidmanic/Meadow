using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.FindPaged)]
public class FindPagedSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public string KeyFindPagedProcedureName => Toolbox.ProcessedType.NameConvention.FindPagedProcedureName;
    public string KeyIndexProcedureName => Toolbox.ProcessedType.NameConvention.IndexEntityProcedureName;
    public string KeyFindPagedProcedureNameFullTree => Toolbox.ProcessedType.NameConvention.FindPagedProcedureNameFullTree;

    public string KeySelectColumns => Toolbox.GetSelectColumns(ColumnNameTranslation.DataOwnerDotColumnName);
    public string KeyTableName => Toolbox.ProcessedType.NameConvention.TableName;
    public string KeyFullTreeView => Toolbox.ProcessedType.NameConvention.FullTreeViewName;
    public string KeySearchIndexTableName => Toolbox.ProcessedType.NameConvention.SearchIndexTableName;
    public string KeyIdFieldName => Toolbox.IdFieldNameOrDefault("Id");
    public string KeyIdFieldNameFullTree => Toolbox.IdFieldNameOrDefaultFullTree("Id");
    public string KeyEntityFilterSegment => Toolbox.GetEntityFiltersWhereClause(" AND ", " ");
    
    public string IndexProcedure(string body)=> Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.IndexEntityProcedureName,body,string.Empty, 
        Toolbox.ProcessedType.NameConvention.TableName,
        Toolbox.Parameters()
            .Add().Name("ResultId").Type(Toolbox.IdFieldTypeOrDefault(string.Empty))
            .Add)
    
    public string Template => @"
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {KeyIndexProcedureName} (@ResultId {KeyIdFieldName},@IndexCorpus TEXT) AS
    UPDATE {KeySearchIndexTableName}  SET IndexCorpus=@IndexCorpus
        WHERE {KeySearchIndexTableName}.ResultId=@ResultId;
    INSERT INTO {KeySearchIndexTableName} (ResultId,IndexCorpus)
            SELECT @ResultId,@IndexCorpus WHERE NOT EXISTS(SELECT * FROM {KeySearchIndexTableName}
            WHERE {KeySearchIndexTableName}.ResultId=@ResultId);
    SELECT * FROM {KeySearchIndexTableName} WHERE ROWID=LAST_INSERT_ROWID();
    SELECT * FROM {KeySearchIndexTableName} WHERE {KeySearchIndexTableName}.ResultId=@ResultId OR ROWID = LAST_INSERT_ROWID() LIMIT 1;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {KeyFindPagedProcedureName}(
                                            @Offset INTEGER,
                                            @Size INTEGER,
                                            @FilterExpression TEXT,
                                            @SearchExpression TEXT,
                                            @OrderExpression TEXT)
AS
    SELECT {KeySelectColumns} FROM {KeyTableName}
    LEFT JOIN {KeySearchIndexTableName} ON {KeyTableName}.{KeyIdFieldName}={KeySearchIndexTableName}.ResultId
    WHERE (&@FilterExpression) AND (&@SearchExpression){KeyEntityFilterSegment}
    ORDER BY &@OrderExpression LIMIT @Offset,@Size;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {KeyFindPagedProcedureNameFullTree}(
                                            @Offset INTEGER,
                                            @Size INTEGER,
                                            @FilterExpression TEXT,
                                            @SearchExpression TEXT,
                                            @OrderExpression TEXT)
AS
    SELECT {KeyFullTreeView}.* FROM {KeyFullTreeView} INNER JOIN 
    (SELECT DISTINCT {KeyIdFieldNameFullTree} 'Id' FROM {KeyFullTreeView}
    LEFT JOIN {KeySearchIndexTableName} ON {KeyFullTreeView}.{KeyIdFieldNameFullTree}={KeySearchIndexTableName}.ResultId
    WHERE (&@FilterExpression) AND (&@SearchExpression) ORDER BY &@OrderExpression LIMIT @Offset,@Size) Prx
    ON {KeyFullTreeView}.{KeyIdFieldNameFullTree}=Prx.Id;
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}