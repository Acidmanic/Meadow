using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

public class EntityDataBoundSnippet : ISnippet
{
    private readonly SnippetToolboxBuilder _builder;

    public EntityDataBoundSnippet(Type entityType,MeadowConfiguration configuration, RepetitionHandling repetitionHandling)
    {
        _builder = new SnippetToolboxBuilder(configuration, entityType);

        _builder.RepetitionHandling(repetitionHandling);
    }

    public SnippetToolbox? Toolbox
    {
        get => _builder.Build();
        set { }
    }

    public string KeyRangeProcedureCreationPhrase
        => Toolbox?.SqlTranslator.CreateProcedurePhrase(Toolbox.Configurations.RepetitionHandling,
            Toolbox.ProcessedType.NameConvention.RangeProcedureName) ?? string.Empty;

    public string KeyExistingValuesProcedureCreationPhrase
        => Toolbox?.SqlTranslator.CreateProcedurePhrase(Toolbox.Configurations.RepetitionHandling,
            Toolbox.ProcessedType.NameConvention.ExistingValuesProcedureName) ?? string.Empty;

    public string KeyTableName => Toolbox?.ProcessedType.NameConvention.TableName ?? string.Empty;

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