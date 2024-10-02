using System.Linq;
using System.Runtime.CompilerServices;
using Meadow.Contracts;
using Meadow.Enums;
using Meadow.Requests.GenericEventStreamRequests.Models;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;
using Meadow.Utility;
using Meadow.ValueObjects;

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

    public ISnippet ReadAllStreamsProcedure => new SelectProcedureSnippet(Builder
        .Order(o => o.OrderAscendingBy(e => e.EventRowNumber))
        .Build(), NameConvention.ReadAllStreams);

    public ISnippet ReadStreamByStreamIdProcedure => new SelectProcedureSnippet(Builder
        .By(ps => ps.Add(e => e.StreamId))
        .Build(), NameConvention.ReadStreamByStreamId);

    public ISnippet ReadAllStreamChunksSelect => new SelectSnippet(Builder.Inline().Build());

    private Parameter BaseEventIdParameter => Toolbox.Parameters(pb => pb.Add().Name("BaseEventId").Type(Toolbox.ProcessedType.EventIdType!)).First();

    private Parameter SizeParameter => Toolbox.Parameters(pb => pb.Add().Name("Count").Type<long>()).First();

    private Parameter EventIdParameter =>
        EntityTypeUtilities.ParameterByAddress<ObjectEntry<object, object>>
        (ProcessedType.EventStreamType, en => en.EventId,
            Toolbox.Construction.MeadowConfiguration,
            Toolbox.TypeNameMapper) ?? Parameter.Null;

    public ISnippet SelectEventRowNumber => new SelectSnippet(
        Builder.Inline()
            .SelectColumns(oe => oe.Add(e => e.EventRowNumber))
            .Filter(f =>
                f.Where(oe => oe.EventId)
                    .IsEqualTo(BaseEventIdParameter))
            .Build());

    public ISnippet ReadAllStreamsChunksProcedure(string selectBaseEvent) => new SelectProcedureSnippet(Builder
        .Filter(fb =>
            fb.Where(oe => oe.EventRowNumber)
                .IsLargerThan(new Code(selectBaseEvent, KnownWraps.Parentheses)))
        .InputParameters(BaseEventIdParameter)
        .Order(p => p.OrderAscendingBy(oe => oe.EventRowNumber))
        .Size(SizeParameter)
        .Build(), NameConvention.ReadChunkProcedureName);
    
    
    public ISnippet ReadStreamChunkByStreamIdProcedure(string selectBaseEvent) => new SelectProcedureSnippet(Builder
        .Filter(fb =>
            fb.Where(oe => oe.EventRowNumber)
                .IsLargerThan(new Code(selectBaseEvent, KnownWraps.Parentheses)))
        .By(ps => ps.Add(oe => oe.StreamId))
        .InputParameters(BaseEventIdParameter)
        .Order(p => p.OrderAscendingBy(oe => oe.EventRowNumber))
        .Size(SizeParameter)
        .Build(), NameConvention.ReadChunkProcedureName);

    public ISnippet Line => new CommentLineSnippet();

    public string Template => @"
{Line}
{InsertProcedure}
{Line}
{ReadAllStreamsProcedure}
{Line}
{ReadStreamByStreamIdProcedure}
{Line}
{ReadAllStreamsChunksProcedure}{SelectEventRowNumber}{/ReadAllStreamsChunksProcedure}
{Line}
{ReadStreamChunkByStreamIdProcedure}{SelectEventRowNumber}{/ReadStreamChunkByStreamIdProcedure}
{Line}
".Trim();
}