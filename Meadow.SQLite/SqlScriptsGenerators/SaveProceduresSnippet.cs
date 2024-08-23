using System.Collections.Generic;
using System.Linq;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.SQLite.SqlScriptsGenerators;

[CommonSnippet(CommonSnippets.SaveProcedure)]
public class SaveProceduresSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }


    public List<ISnippet> SaveByCollectionNames => CreateSaveSnippets();


    public string Template => $"{{{nameof(SaveByCollectionNames)}}}";


    private List<ISnippet> CreateSaveSnippets()
    {
        var snippets = new List<ISnippet>();
        
        if (Toolbox is { } toolbox)
        {
            snippets.Add(new TitleBarSnippet($"Save Procedures For Type: {toolbox.EntityType.Name}:{toolbox.EffectiveType.Name} Table: {toolbox.ProcessedType.NameConvention.TableName}"));
            
            var profile = toolbox.ProcessedType.RecordIdentificationProfile;

            foreach (var collectiveIdSet in profile.CollectiveIdentifiersByName)
            {
                var analysis = toolbox.ComponentsProcessor
                    .CreateSaveProcedureAnalysis(collectiveIdSet.Key, collectiveIdSet.Value.ToArray());
                
                snippets.Add(new SaveByCollectionNameSnippet(toolbox,analysis));
                
                snippets.Add(new CommentLineSnippet());
            }
            foreach (var singularIdSet in profile.SingularIdentifiersByName)
            {
                var analysis = toolbox.ComponentsProcessor
                    .CreateSaveProcedureAnalysis(singularIdSet.Key, singularIdSet.Value);
                
                snippets.Add(new SaveByCollectionNameSnippet(toolbox,analysis));
                
                snippets.Add(new CommentLineSnippet());
            }
        }

        return snippets;
    }
  
}

public class SaveByCollectionNameSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }

    public SaveByCollectionNameSnippet(SnippetToolbox toolbox,SaveProcedureComponents saveComponents)
    {
        Toolbox = toolbox;
        KeyProcedureName = saveComponents.ProcedureName;
        PreKeyWhereClause =toolbox.ParameterNameValueSetJoint(saveComponents.WhereEqualities, " AND ", "@");
        PreKeyInsertColumns=string.Join(',', saveComponents.InsertUpdateParameters.Select(p => p.Name));
        PreKeyInsertValues=string.Join(',', saveComponents.InsertUpdateParameters.Select(p => "@" + p.Name));
        PreKeyUpdates=toolbox.ParameterNameValueSetJoint(saveComponents.InsertUpdateParameters, ",", "@");
        KeyParameters=toolbox.ParameterNameTypeJoint(toolbox.ProcessedType.Parameters, ",", "@");
    }

    public string KeyHeaderCreation
    {
        get
        {
            var creationHeader = "CREATE PROCEDURE";

            if (Toolbox is { } toolbox)
            {
                if (toolbox.Configurations.RepetitionHandling == RepetitionHandling.Skip)
                {
                    creationHeader = "CREATE IF NOT EXISTS";
                }

                if (toolbox.Configurations.RepetitionHandling == RepetitionHandling.Alter)
                {
                    creationHeader = "CREATE OR ALTER";
                }
            }

            return creationHeader;
        }
    }

    public string KeyProcedureName { get; }
    
    public string PreKeyWhereClause { get; }
    
    public string PreKeyInsertColumns { get; }
    
    public string PreKeyInsertValues { get; }
    
    public string PreKeyUpdates { get; }

    public string KeyTableName => Toolbox?.ProcessedType.NameConvention.TableName ?? "";

    public string KeyEntityFilterSegment => Toolbox?.GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly, " AND ", " ") ??"";

    public string KeyParameters { get; }
    
    public string Template => $@"
{KeyHeaderCreation} {KeyProcedureName} ({KeyParameters}) AS

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