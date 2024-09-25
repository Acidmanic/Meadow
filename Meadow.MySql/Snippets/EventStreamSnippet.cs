using System;
using System.Linq;
using Meadow.Contracts;
using Meadow.Extensions;
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

    // private EventStreamPreferencesInfo Info =>
    //     EventStreamPreferencesInfo.FromType(Toolbox.ProcessedType.EntityType);
    //
    // private Type ObjectEntryType => typeof(ObjectEntry<,>).MakeGenericType(Info.EventIdType, Info.StreamIdType);
    //
    public string InsertProcedure => Toolbox.TranslateEventStreamsPhraseInsertProcedure();
    //
    public string ReadAllStreamsProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.ReadAllStreams, body);
    
    public string ReadStreamByStreamIdProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.ReadStreamByStreamId,
        p => p
            .Name("StreamId").Type(Toolbox.ProcessedType.StreamIdTypeName ?? ""), body);
    
    public ISnippet SelectReadAllStreamsSelect => SelectAllSnippet.Create<ObjectEntry<object, object>>
        (Toolbox.ProcessedType.EventStreamType, null, 
            o => o.OrderAscendingBy(oe => oe.EventRowNumber),
            false, b => b.OverrideDbObjectName(Toolbox.ProcessedType.NameConvention.EventStreamTableName)); 
    
    
    public ISnippet ReadStreamByStreamIdSelect => SelectAllSnippet.Create<ObjectEntry<object, object>>
    (Toolbox.ProcessedType.EventStreamType,
        b => b.Where(e => e.StreamId).IsEqualTo(10), 
        o => o.OrderAscendingBy(oe => oe.EventRowNumber),
        false, b => b.OverrideDbObjectName(Toolbox.ProcessedType.NameConvention.EventStreamTableName));
    
    
    public string Template => @"
-- ---------------------------------------------------------------------------------------------------------------------
{InsertProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadAllStreamsProcedure}
    {SelectReadAllStreamsSelect}
{/ReadAllStreamsProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadStreamByStreamIdProcedure}
    {ReadStreamByStreamIdSelect}
{/ReadStreamByStreamIdProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}