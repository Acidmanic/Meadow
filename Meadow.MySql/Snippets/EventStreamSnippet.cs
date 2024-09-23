using System;
using Meadow.Requests.GenericEventStreamRequests.Models;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.MySql.Snippets;

[CommonSnippet(CommonSnippets.EventStreamScript)]
public class EventStreamSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public string KeyTableName => Toolbox.ProcessedType.NameConvention.EventStreamTableName;

    public string KeyEventIdDefinition => Toolbox.EventIdDefinitionPhrase(",");

    public string KeyReadStreamChunkByStreamIdProcedureName =>
        Toolbox.ProcessedType.NameConvention.ReadStreamChunkByStreamId;

    private string DefaultTypeName => Toolbox.TypeNameMapper.GetDatabaseTypeName(typeof(string));

    public string KeyStreamIdType => Toolbox.ProcessedType.StreamIdTypeName ?? DefaultTypeName;
    public string KeyTypeNameType => Toolbox.ProcessedType.EventStreamTypeNameDatabaseType ?? DefaultTypeName;
    public string KeyAssemblyNameType => Toolbox.ProcessedType.EventStreamAssemblyNameDatabaseType ?? DefaultTypeName;

    public string KeySerializedValueType => Toolbox.ProcessedType.EventStreamSerializedValueDatabaseType ?? DefaultTypeName;


    // private Action<IParameterBuilder> EventTableInsertParameterSetup => p => p
    //     .Name("StreamId").Type(Toolbox.ProcessedType.StreamIdTypeName ?? DefaultTypeName).Add()
    //     .Name("EventId").Type(Toolbox.ProcessedType.EventIdTypeName ?? DefaultTypeName).Add()
    //     .Name("TypeName").Type(Toolbox.ProcessedType.EventStreamTypeNameDatabaseType ?? DefaultTypeName).Add()
    //     .Name("AssemblyName").Type(Toolbox.ProcessedType.EventStreamAssemblyNameDatabaseType ?? DefaultTypeName).Add()
    //     .Name("SerializedValue").Type(Toolbox.ProcessedType.EventStreamSerializedValueDatabaseType ?? DefaultTypeName);

    // private Action<IParameterBuilder> EventTableParameterSetup => p =>
    // {
    //     EventTableInsertParameterSetup(p);
    //
    //         p.Add().Name("EventRowNumber").Type<int>();
    // };


    // public string InsertProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
    //     Toolbox.ProcessedType.NameConvention.InsertEvent,
    //     EventTableInsertParameterSetup
    //     , body);

    public ISnippet EventTable => 
        Toolbox.ProcessedType.IsStreamEvent ? new CustomTableSnippet(
        typeof(ObjectEntry<,>).MakeGenericType(Toolbox.ProcessedType.EventStream!.StreamIdType, 
            Toolbox.ProcessedType.EventStream.StreamIdType),
        Toolbox.ProcessedType.NameConvention.EventStreamTableName):
            new TitleBarSnippet(Toolbox.ProcessedType.NameConvention.EntityType.FullName + " is not pointing to any event streams, please use Event Stream annotations to annotate the type."); 
    //Toolbox.TranslateTable(EventTableParameterSetup, KeyTableName);

    // public string ReadStreamChunkByStreamIdProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
    //     Toolbox.ProcessedType.NameConvention.ReadStreamChunkByStreamId,
    //     p => p
    //         .Name("StreamId").Type(Toolbox.ProcessedType.StreamIdTypeName ?? "").Add()
    //         .Name("BaseEventId").Type(Toolbox.ProcessedType.EventIdTypeName ?? "").Add()
    //         .Name("Count").Type<int>()
    //     , body);
    //
    // public string ReadStreamByStreamIdProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
    //     Toolbox.ProcessedType.NameConvention.ReadStreamByStreamId,
    //     p => p
    //         .Name("StreamId").Type(Toolbox.ProcessedType.StreamIdTypeName ?? ""), body);
    //
    // public string ReadAllStreamsProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
    //     Toolbox.ProcessedType.NameConvention.ReadAllStreams, body);
    //
    //
    // public string ReadAllStreamsChunksProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
    //     Toolbox.ProcessedType.NameConvention.ReadAllStreamsChunks,
    //     p => p
    //         .Name("BaseEventId").Type(Toolbox.ProcessedType.EventIdTypeName ?? "").Add().Name("Count").Type<int>()
    //     , body);
    //

    public string Template => @"
-- ---------------------------------------------------------------------------------------------------------------------
{EventTable}
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}