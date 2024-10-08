using System;
using Meadow.Configuration;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

public class EntityDataBoundSnippet : ISnippet
{
    
    public ISnippetToolbox Toolbox { get; set; }

    public EntityDataBoundSnippet(Type entityType,MeadowConfiguration configuration, RepetitionHandling repetitionHandling)
    {
        var builder = new SnippetToolboxBuilder(configuration, entityType);

        builder.RepetitionHandling(repetitionHandling);

        Toolbox = builder.Build();
    }

    public string KeyTableName => Toolbox.ProcessedType.NameConvention.TableName;

    private string Procedure(string body, string name) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        name, pb => pb.Name("FieldName").Type<string>(), body, string.Empty, string.Empty);

    public string RangeProcedure(string body) => Procedure(body, Toolbox.ProcessedType.NameConvention.RangeProcedureName);
    public string ExistingProcedure(string body) => Procedure(body, Toolbox.ProcessedType.NameConvention.ExistingValuesProcedureName);
    
    public string Template => @"
-- ---------------------------------------------------------------------------------------------------------------------
{RangeProcedure}
    SELECT MAX(&@FieldName) 'Max', MIN(&@FieldName) 'Min' FROM {KeyTableName};
{/RangeProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ExistingProcedure}
    SELECT DISTINCT &@FieldName 'Value' FROM {KeyTableName} ORDER BY &@FieldName ASC;
{/ExistingProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}