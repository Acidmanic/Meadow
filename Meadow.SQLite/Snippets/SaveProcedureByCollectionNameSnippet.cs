using System.Linq;
using Meadow.Contracts;
using Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

public class SaveProcedureByCollectionNameSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }

    public SaveProcedureByCollectionNameSnippet(SnippetToolbox toolbox, SaveProcedureComponents saveComponents)
    {
        Toolbox = toolbox;

        KeyProcedureCreationHeader = toolbox.SqlTranslator.CreateProcedurePhrase
            (toolbox.Configurations.RepetitionHandling, saveComponents.ProcedureName);
        
        PreKeyWhereClause = toolbox.ParameterNameValueSetJoint(saveComponents.WhereEqualities, " AND ", "@");
        PreKeyInsertColumns = string.Join(',', saveComponents.InsertUpdateParameters.Select(p => p.Name));
        PreKeyInsertValues = string.Join(',', saveComponents.InsertUpdateParameters.Select(p => "@" + p.Name));
        PreKeyUpdates = toolbox.ParameterNameValueSetJoint(saveComponents.InsertUpdateParameters, ",", "@");
        KeyParameters = toolbox.ParameterNameTypeJoint(toolbox.ProcessedType.Parameters, ",", "@");
    }

    public string KeyProcedureCreationHeader { get; }
    
    public string PreKeyWhereClause { get; }

    public string PreKeyInsertColumns { get; }

    public string PreKeyInsertValues { get; }

    public string PreKeyUpdates { get; }

    public string KeyTableName => Toolbox?.ProcessedType.NameConvention.TableName ?? "";

    public string KeyEntityFilterSegment => Toolbox?.GetEntityFiltersWhereClause( " AND ", " ") ?? "";

    public string KeyParameters { get; }

    public string Template => $@"
{KeyProcedureCreationHeader}({KeyParameters}) AS

    BEGIN;
    PRAGMA temp_store = 2;
    CREATE TEMP TABLE _alreadyExists(Id INTEGER PRIMARY KEY);
    INSERT INTO _alreadyExists (Id) SELECT (1) WHERE EXISTS(SELECT * FROM {KeyTableName} WHERE {PreKeyWhereClause}{KeyEntityFilterSegment});

    UPDATE {KeyTableName} SET {PreKeyUpdates} WHERE {PreKeyWhereClause}{KeyEntityFilterSegment} AND EXISTS (SELECT * FROM _alreadyExists);

    INSERT INTO {KeyTableName} ({PreKeyInsertColumns}) SELECT {PreKeyInsertValues} WHERE NOT EXISTS (SELECT * FROM _alreadyExists);

    SELECT * FROM {KeyTableName} WHERE (({PreKeyWhereClause}) OR ROWID = LAST_INSERT_ROWID()) {KeyEntityFilterSegment} LIMIT 1;
    DROP TABLE _alreadyExists;
    END;
GO
".Trim();
}