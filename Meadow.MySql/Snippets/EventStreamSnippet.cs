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
    
    public ISnippet ReadAllStreamsProcedure => new ReadAllProcedureSnippet(Builder
        .Order(o => o.OrderAscendingBy(e => e.EventRowNumber))
        .Build(),NameConvention.ReadAllStreams);

    public ISnippet ReadStreamByStreamIdProcedure => new ReadAllProcedureSnippet(Builder
        .By(ps => ps.Add(e => e.StreamId))
        .Build(),NameConvention.ReadStreamByStreamId);

    public ISnippet ReadAllStreamChunksSelect => new ReadAllSelectSnippet(Builder.Inline().Build());
    
    
    private Parameter BaseEventIdParameter {
        get
        {
            var b = new ParameterBuilder(Toolbox.TypeNameMapper);
            
            b.Add().Name("BaseEventId").Type(Toolbox.ProcessedType.EventIdType!);

            return b.Build().First();
        }
    }
        
    
    
    private Parameter EventIdParameter =>
        EntityTypeUtilities.ParameterByAddress<ObjectEntry<object, object>>
        (ProcessedType.EventStreamType, en => en.EventId,
            Toolbox.Construction.MeadowConfiguration,
            Toolbox.TypeNameMapper) ?? Parameter.Null;
    
    public ISnippet SelectEventRowNumber => new ReadAllSelectSnippet(
        Builder.Inline()
            .SelectColumns(oe => oe.Add(e=>e.EventRowNumber))
            .Filter(f => 
                f.Where(oe => oe.EventId)
                .IsEqualTo(BaseEventIdParameter))
            .Build());
    
    
    //.By(ps => ps.Add(e => e.StreamId))
    public ISnippet ReadAllStreamsChunksProcedure(string selectBaseEvent) => new ReadAllProcedureSnippet(Builder
        .Filter(fb => 
            fb.Where(oe => oe.EventId)
                .IsLargerThan(new Code(selectBaseEvent,KnownWraps.Parentheses)))
        .InputParameters(BaseEventIdParameter)
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
{ReadAllStreamsChunksProcedure}{SelectEventRowNumber}{/ReadAllStreamsChunksProcedure}
{Line}
".Trim();
}