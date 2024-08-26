using System;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;


[CommonSnippet(CommonSnippets.FindPaged)]
public class FindPagedSnippet:ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }

    public string KeyFindPagedProcedureName => T(t => t.ProcessedType.NameConvention.FindPagedProcedureName);
    public string KeyFindPagedProcedureNameFullTree => T(t => t.ProcessedType.NameConvention.FindPagedProcedureNameFullTree);

    public string KeySelectColumns => T(t => t.GetSelectColumns());
    public string KeyTableName => T(t => t.ProcessedType.NameConvention.TableName);
    public string KeyFullTreeView => T(t => t.ProcessedType.NameConvention.FullTreeViewName);
    public string KeySearchIndexTableName => T(t => t.ProcessedType.NameConvention.SearchIndexTableName);
    public string KeyIdFieldName => T(t => t.IdFieldNameOrDefault("Id"));
    public string KeyIdFieldNameFullTree => T(t => t.IdFieldNameOrDefaultFullTree("Id"));
    public string KeyEntityFilterSegment => T(t => t.GetEntityFiltersWhereClause(" AND "," "));
    
    private string T(Func<SnippetToolbox, string> pickValue)
    {
        if (Toolbox is { } toolbox)
        {
            return pickValue(toolbox);
        }

        return string.Empty;
    }
   
        
    public string Template => $@"
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