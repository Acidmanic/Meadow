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
    public string KeyFindPagedProcedureNameFullTree => Toolbox.ProcessedType.NameConvention.FindPagedProcedureNameFullTree;

    public string KeySelectColumns => Toolbox.GetSelectColumns(ColumnNameTranslation.DataOwnerDotColumnName);
    public string KeyTableName => Toolbox.ProcessedType.NameConvention.TableName;
    public string KeyFullTreeView => Toolbox.ProcessedType.NameConvention.FullTreeViewName;
    public string KeySearchIndexTableName => Toolbox.ProcessedType.NameConvention.SearchIndexTableName;
    public string KeyIdFieldName => Toolbox.IdFieldNameOrDefault("Id");
    public string KeyIdFieldNameFullTree => Toolbox.IdFieldNameOrDefaultFullTree("Id");
    public string KeyEntityFilterSegment => Toolbox.GetEntityFiltersWhereClause(" AND ", " ");

    public string Template => @"
-- ---------------------------------------------------------------------------------------------------------------------
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