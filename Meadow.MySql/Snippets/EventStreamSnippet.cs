using Meadow.Contracts;
using Meadow.Requests.GenericEventStreamRequests.Models;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.MySql.Snippets;

[CommonSnippet(CommonSnippets.EventStreamScript)]
public class EventStreamSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    private ProcessedType ProcessedType => Toolbox.ProcessedType;
    private NameConvention NameConvention => Toolbox.ProcessedType.NameConvention;

    private RepetitionHandling RepetitionHandling => Toolbox.Configurations.RepetitionHandling;

    public string InsertProcedure => Toolbox.TranslateEventStreamsPhraseInsertProcedure();
    
    private ReadAllProcedureSnippetBuilder<ObjectEntry<object, object>> Builder =>
        new ReadAllProcedureSnippetBuilder<ObjectEntry<object, object>>
                (Toolbox.ProcessedType.NameConvention.ReadAllStreams, Toolbox)
            .EntityType(ProcessedType.EventStreamType)
            .ManipulateConfigurations(cb =>
                cb.OverrideDbObjectName(Toolbox.ProcessedType.NameConvention.EventStreamTableName));
    
    public ISnippet ReadAllStreamsProcedure => Builder
        .Order(o => o.OrderAscendingBy(e => e.EventRowNumber))
        .Build();

    public ISnippet ReadStreamByStreamIdProcedure => Builder
        .By(ps => ps.Add(e => e.StreamId))
        .Build();

    public ISnippet ReadAllStreamsChunksProcedure => Builder
        .By(ps => ps.Add(e => e.StreamId))
        .Source(Builder.Build())
        .Build();

    public string Template => @"
-- ---------------------------------------------------------------------------------------------------------------------
{InsertProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadAllStreamsProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadStreamByStreamIdProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadAllStreamsChunksProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}