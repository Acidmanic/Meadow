using System.Linq;
using Meadow.Models;
using Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;
using Meadow.Sql.Extensions;

namespace Meadow.MySql.Snippets;

public class SaveProcedureByCollectionNameSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; }

    private readonly SaveProcedureComponents _components;

    public SaveProcedureByCollectionNameSnippet(ISnippetToolbox toolbox, SaveProcedureComponents saveComponents)
    {
        Toolbox = toolbox;
        _components = saveComponents;

        PreKeyWhereClause = toolbox.EqualityClause(null,false,saveComponents.WhereEqualities.ToArray());
        PreKeyInsertColumns = string.Join(',', saveComponents.InsertUpdateParameters.Select(p => toolbox.SqlTranslator.Decorate(p,ParameterUsage.ColumnName)));
        PreKeyInsertValues = string.Join(',', saveComponents.InsertUpdateParameters.Select(p => toolbox.SqlTranslator.Decorate(p,ParameterUsage.ProcedureBody)));
        PreKeyUpdates = toolbox.ParameterNameValueSetPair(saveComponents.InsertUpdateParameters, ",");
    }
    
    public string PreKeyWhereClause { get; }

    public string PreKeyInsertColumns { get; }

    public string PreKeyInsertValues { get; }

    public string PreKeyUpdates { get; }

    public string KeyTableName => Toolbox.ProcessedType.NameConvention.TableName;

    public string KeyEntityFilterSegment => Toolbox.GetEntityFiltersWhereClause(" AND ", " ");
    
    public string Procedure(string content) => Toolbox.Procedure(
        Toolbox.Configurations.RepetitionHandling,
        _components.ProcedureName,
        content,
        string.Empty, Toolbox.ProcessedType.NameConvention.TableName,
        Toolbox.ProcessedType.Parameters.ToArray());

    public string Template => @"
{Procedure}
    BEGIN;
    PRAGMA temp_store = 2;
    CREATE TEMP TABLE _alreadyExists(Id INTEGER PRIMARY KEY);
    INSERT INTO _alreadyExists (Id) SELECT (1) WHERE EXISTS(SELECT * FROM {KeyTableName} WHERE {PreKeyWhereClause}{KeyEntityFilterSegment});

    UPDATE {KeyTableName} SET {PreKeyUpdates} WHERE {PreKeyWhereClause}{KeyEntityFilterSegment} AND EXISTS (SELECT * FROM _alreadyExists);

    INSERT INTO {KeyTableName} ({PreKeyInsertColumns}) SELECT {PreKeyInsertValues} WHERE NOT EXISTS (SELECT * FROM _alreadyExists);

    SELECT * FROM {KeyTableName} WHERE (({PreKeyWhereClause}) OR ROWID = LAST_INSERT_ROWID()) {KeyEntityFilterSegment} LIMIT 1;
    DROP TABLE _alreadyExists;
    END;
{/Procedure}
".Trim();
}