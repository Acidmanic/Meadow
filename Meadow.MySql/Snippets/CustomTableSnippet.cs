using System;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;
using Meadow.Utility;

namespace Meadow.MySql.Snippets;

public class CustomTableSnippet : ISnippet
{
    private readonly Type _entityType;
    private readonly string? _overrideTableName;

    public CustomTableSnippet(Type entityType, string? overrideTableName = null)
    {
        _overrideTableName = overrideTableName;
        _entityType = entityType;
    }

    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public ISnippet Split => new SplitSnippet();

    public string Table
    {
        get
        {
            var processedType = EntityTypeUtilities.Process(_entityType, Toolbox.Construction.MeadowConfiguration, Toolbox.TypeNameMapper);

            var toolbox = new SnippetToolboxBuilder(Toolbox.Construction.MeadowConfiguration, _entityType)
                .RepetitionHandling(Toolbox.Configurations.RepetitionHandling)
                .Build();
            
            return toolbox.TranslateTable(processedType.Parameters, _overrideTableName);
        }
    }

    public string Template => "{Table}\n{Split}";
}