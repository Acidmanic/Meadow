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
    
    private SelectSnippetParametersBuilder<ObjectEntry<object, object>> Builder =>
        new SelectSnippetParametersBuilder<ObjectEntry<object, object>>(Toolbox)
            .EntityType(ProcessedType.EventStreamType)
            .ManipulateConfigurations(cb =>
                cb.OverrideDbObjectName(Toolbox.ProcessedType.NameConvention.EventStreamTableName));
    
    public ISnippet ReadAllStreamsProcedure => new ReadAllProcedureSnippet(Builder
        .Order(o => o.OrderAscendingBy(e => e.EventRowNumber))
        .Build(),NameConvention.ReadAllStreams);

    public ISnippet ReadStreamByStreamIdProcedure => new ReadAllProcedureSnippet(Builder
        .By(ps => ps.Add(e => e.StreamId))
        .Build(),NameConvention.ReadStreamByStreamId);

    private ISnippet ReadAllStreamChunksSelect => new ReadAllSelectSnippet(Builder.Inline().Build());
    
    public ISnippet ReadAllStreamsChunksProcedure => new ReadAllProcedureSnippet(Builder
        .By(ps => ps.Add(e => e.StreamId))
        .Source(ReadAllStreamChunksSelect,"Source")
        .Build(),NameConvention.ReadChunkProcedureName);

    public ISnippet Line => new CommentLineSnippet();
    
    public string Template => @"
{Line}
{InsertProcedure}
{Line}
{ReadAllStreamsProcedure}
{Line}
{ReadStreamByStreamIdProcedure}
{Line}
{ReadAllStreamsChunksProcedure}
{Line}
".Trim();
}