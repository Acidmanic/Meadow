using System.Linq;
using Meadow.Extensions;
using Meadow.Models;
using Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;

namespace Meadow.MySql.Snippets;

public class SaveProcedureByCollectionNameSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; }

    private readonly SaveProcedureComponents _components;

    public SaveProcedureByCollectionNameSnippet(ISnippetToolbox toolbox, SaveProcedureComponents saveComponents)
    {
        Toolbox = toolbox;
        _components = saveComponents;

        PreKeyWhereByIdentifiers = toolbox.EqualityClause(null,false,saveComponents.WhereEqualities.ToArray());
        PreKeyInsertColumns = string.Join(',', saveComponents.InsertUpdateParameters.Select(p => toolbox.SqlTranslator.Decorate(p,ParameterUsage.ColumnName)));
        PreKeyInsertValues = string.Join(',', saveComponents.InsertUpdateParameters.Select(p => toolbox.SqlTranslator.Decorate(p,ParameterUsage.ProcedureBody)));
        PreKeyUpdates = toolbox.ParameterNameValueSetPair(saveComponents.InsertUpdateParameters, ",");
    }
    
    public string PreKeyWhereByIdentifiers { get; }

    public string PreKeyInsertColumns { get; }

    public string PreKeyInsertValues { get; }

    public string PreKeyUpdates { get; }

    public string KeyTableName => Toolbox.ProcessedType.NameConvention.TableName;
    
    private static readonly string NewIdVariable = "@nid";
    
    public string KeyWhereForInserted => Toolbox.ProcessedType.HasId ? 
        Toolbox.EqualityClause(Toolbox.ProcessedType.IdParameter!, NewIdVariable) : 
        PreKeyWhereByIdentifiers;


    public string KeyEntityFilterSegment => Toolbox.GetEntityFiltersWhereClause(" AND ", " ");
    
    public string Procedure(string content) => Toolbox.Procedure(
        Toolbox.Configurations.RepetitionHandling,
        _components.ProcedureName,
        content,
        string.Empty, Toolbox.ProcessedType.NameConvention.TableName,
        Toolbox.ProcessedType.Parameters.ToArray());

    public string Semicolon => Toolbox.Semicolon();
    
    public string KeyDeclareNewId => Toolbox.ProcessedType.HasId?
        ($"SET {NewIdVariable} = (select LAST_INSERT_ID())"+Toolbox.Semicolon()):string.Empty;

    public string Template => @"
{Procedure}
    IF EXISTS(SELECT 1 FROM {KeyTableName} WHERE {PreKeyWhereByIdentifiers}{KeyEntityFilterSegment}) then
        
        UPDATE {KeyTableName} SET {PreKeyUpdates} WHERE {PreKeyWhereByIdentifiers}{KeyEntityFilterSegment}{Semicolon}
        
        SELECT * FROM {KeyTableName} WHERE {PreKeyWhereByIdentifiers}{KeyEntityFilterSegment} LIMIT 1{Semicolon}
        
    ELSE

        INSERT INTO {KeyTableName} ({PreKeyInsertColumns}) VALUES ({PreKeyInsertValues}){Semicolon}

        {KeyDeclareNewId}

        SELECT * FROM {KeyTableName} WHERE {KeyWhereForInserted}{KeyEntityFilterSegment}{Semicolon}
    END IF;
{/Procedure}
".Trim();
}