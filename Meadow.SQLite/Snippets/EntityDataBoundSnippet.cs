using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

public class EntityDataBoundSnippet : ISnippet
{
    private readonly SnippetToolbox _toolbox;

    public EntityDataBoundSnippet(Type entityType,MeadowConfiguration configuration, RepetitionHandling repetitionHandling)
    {
        var builder = new SnippetToolboxBuilder(configuration, entityType);

        builder.RepetitionHandling(repetitionHandling);

        _toolbox = builder.Build();
    }

    public SnippetToolbox? Toolbox
    {
        get => _toolbox;
        set { }
    }

    public string KeyRangeProcedureCreationPhrase
        => _toolbox.SqlTranslator.CreateProcedurePhrase(_toolbox.Configurations.RepetitionHandling,
            _toolbox.ProcessedType.NameConvention.RangeProcedureName);

    public string KeyExistingValuesProcedureCreationPhrase
        => _toolbox.SqlTranslator.CreateProcedurePhrase(_toolbox.Configurations.RepetitionHandling,
            _toolbox.ProcessedType.NameConvention.ExistingValuesProcedureName);

    public string KeyTableName => _toolbox.ProcessedType.NameConvention.TableName;

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