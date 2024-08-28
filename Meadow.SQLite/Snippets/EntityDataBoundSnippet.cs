using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

public class EntityDataBoundSnippet : ISnippet
{
    
    public SnippetToolbox Toolbox { get; set; }

    public EntityDataBoundSnippet(Type entityType,MeadowConfiguration configuration, RepetitionHandling repetitionHandling)
    {
        var builder = new SnippetToolboxBuilder(configuration, entityType);

        builder.RepetitionHandling(repetitionHandling);

        Toolbox = builder.Build();
    }

    public string KeyRangeProcedureCreationPhrase
        => Toolbox.SqlTranslator.CreateProcedurePhrase(RepetitionHandling.Alter,
            Toolbox.ProcessedType.NameConvention.RangeProcedureName);

    public string KeyExistingValuesProcedureCreationPhrase
        => Toolbox.SqlTranslator.CreateProcedurePhrase(RepetitionHandling.Alter,
            Toolbox.ProcessedType.NameConvention.ExistingValuesProcedureName);

    public string KeyTableName => Toolbox.ProcessedType.NameConvention.TableName;

    public string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
{{{nameof(KeyRangeProcedureCreationPhrase)}}}(@FieldName TEXT) AS
    SELECT MAX(&@FieldName) 'Max', MIN(&@FieldName) 'Min' FROM {{{nameof(KeyTableName)}}};
GO
-- ---------------------------------------------------------------------------------------------------------------------
{{{nameof(KeyExistingValuesProcedureCreationPhrase)}}}(@FieldName TEXT) AS
    SELECT DISTINCT &@FieldName 'Value' FROM {{{nameof(KeyTableName)}}} ORDER BY &@FieldName ASC;
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}