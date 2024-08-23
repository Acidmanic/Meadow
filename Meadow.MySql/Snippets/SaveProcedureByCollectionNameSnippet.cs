using System.Linq;
using Meadow.Contracts;
using Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Scaffolding.Snippets;

namespace Meadow.MySql.Snippets;

public class SaveProcedureByCollectionNameSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }

    public SaveProcedureByCollectionNameSnippet(SnippetToolbox toolbox, SaveProcedureComponents saveComponents)
    {
        Toolbox = toolbox;
        KeyProcedureName = saveComponents.ProcedureName;
        PreKeyWhereClause = toolbox.ParameterNameValueSetJoint(saveComponents.WhereEqualities, " AND ", "@");
        PreKeyWhereClause = string.Join("AND ",
            saveComponents.WhereEqualities.Select(p => EqualityClause(KeyTableName, p)));
        PreKeyInsertColumns = string.Join(',', saveComponents.InsertUpdateParameters.Select(p => p.Name));
        PreKeyInsertValues = string.Join(',', saveComponents.InsertUpdateParameters.Select(p => p.Name));
        PreKeyUpdates = toolbox.ParameterNameValueSetJoint(saveComponents.InsertUpdateParameters, ",", string.Empty);
        KeyParameters = toolbox.ParameterNameTypeJoint(toolbox.ProcessedType.Parameters, ",", "IN ");
    }

    private string EqualityClause(string tableName, Parameter p)
    {
        return tableName + "." + p.Name + " " + EqualityAssertion(p) + " " + p.Name;
    }

    private string EqualityAssertion(Parameter p)
    {
        return IsString(p) ? "like" : "=";
    }

    private bool IsString(Parameter p)
    {
        var typeLower = p.Type.ToLower().Trim();

        return typeLower.StartsWith("text") ||
               typeLower.StartsWith("varchar") ||
               typeLower.StartsWith("nvarchar");
    }

    public string KeyHeaderCreation
    {
        get
        {
            if (Toolbox is { } toolbox && toolbox.Configurations.RepetitionHandling == RepetitionHandling.Alter)
            {
                return "DROP PROCEDURE IF EXISTS " + KeyProcedureName + ";" +
                       "\nCREATE PROCEDURE";
            }

            return "CREATE PROCEDURE";
        }
    }

    public string KeyProcedureName { get; }

    public string PreKeyWhereClause { get; }

    public string PreKeyInsertColumns { get; }

    public string PreKeyInsertValues { get; }

    public string PreKeyUpdates { get; }

    public string KeyTableName => Toolbox?.ProcessedType.NameConvention.TableName ?? "";

    public string KeyEntityFilterSegment =>
        Toolbox?.GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly, " AND ", " ") ?? "";

    public string KeyParameters { get; }

    public string Template => $@"
{KeyHeaderCreation} {KeyProcedureName}({KeyParameters})
BEGIN
    IF EXISTS(SELECT 1 FROM {KeyTableName} WHERE {PreKeyWhereClause}{KeyEntityFilterSegment}) THEN
        
        UPDATE {KeyTableName} SET {PreKeyUpdates} WHERE {PreKeyWhereClause}{KeyEntityFilterSegment};
    ELSE
        INSERT INTO {KeyTableName} ({PreKeyInsertColumns}) VALUES ({PreKeyInsertValues});        
    END IF;

    SELECT * FROM {KeyTableName} WHERE {PreKeyWhereClause}{KeyEntityFilterSegment} LIMIT 1;
END;
".Trim();
}