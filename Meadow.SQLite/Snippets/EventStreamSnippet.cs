using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.EventStreamScript)]
public class EventStreamSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public string KeyTableName => Toolbox.ProcessedType.NameConvention.EventStreamTableName;
    public string KeyTypeNameType => Toolbox.ProcessedType.EventStreamTypeNameDatabaseType ?? string.Empty;

    public string KeyEventIdDefinition => Toolbox.EventIdDefinitionPhrase(",");

    public string KeyReadStreamChunkByStreamIdProcedureName =>
        Toolbox.ProcessedType.NameConvention.ReadStreamChunkByStreamId;


    public string InsertProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.InsertEvent,
        p => p
            .Name("StreamId").Type(Toolbox.ProcessedType.StreamIdTypeName ?? "").Add()
            .Name("TypeName").Type(Toolbox.ProcessedType.EventStreamSerializedValueDatabaseType ?? "").Add()
            .Name("AssemblyName").Type(Toolbox.ProcessedType.EventStreamAssemblyNameDatabaseType ?? "").Add()
            .Name("SerializedValue").Type(Toolbox.ProcessedType.EventStreamSerializedValueDatabaseType ?? "")
        , body);

    public string ReadStreamChunkByStreamIdProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.InsertEvent,
        p => p
            .Name("StreamId").Type(Toolbox.ProcessedType.StreamIdTypeName ?? "").Add()
            .Name("BaseEventId").Type(Toolbox.ProcessedType.EventIdTypeName ?? "").Add()
            .Name("Count").Type<int>()
        , body);

    public string ReadStreamByStreamIdProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.ReadStreamByStreamId,
        p => p
            .Name("StreamId").Type(Toolbox.ProcessedType.StreamIdTypeName ?? ""), body);

    public string ReadAllStreamsProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.ReadAllStreams, body);


    public string ReadAllStreamsChunksProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.ReadAllStreamsChunks,
        p => p
            .Name("BaseEventId").Type(Toolbox.ProcessedType.EventIdTypeName ?? "").Add().Name("Count").Type<int>()
        , body);


    public string Template => @"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE {KeyTableName} (
    {KeyEventIdDefinition}
    StreamId {KeyStreamIdType},
    TypeName {KeyTypeNameType},
    AssemblyName {KeyAssemblyNameType},
    SerializedValue {KeySerializedValueType},
    EventRowNumber INTEGER NOT NULL);
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
{InsertProcedure}
    INSERT INTO {KeyTableName} (EventId,StreamId, TypeName,AssemblyName, SerializedValue,EventRowNumber) 
        VALUES (@EventId,@StreamId,@TypeName,@AssemblyName,@SerializedValue,(select (Count(*)+1) from {KeyTableName}) );
    
    SELECT * FROM {KeyTableName} WHERE ROWID=LAST_INSERT_ROWID(); 
{/InsertProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadAllStreamsProcedure}
    SELECT * FROM {KeyTableName} ORDER BY EventRowNumber ASC;
{/ReadAllStreamsProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadStreamByStreamIdProcedure}
    SELECT * FROM {KeyTableName} WHERE {KeyTableName}.StreamId = @StreamId ORDER BY EventRowNumber ASC;
{/ReadStreamByStreamIdProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadAllStreamsChunksProcedure}
    SELECT * FROM {KeyTableName} WHERE
                  EventRowNumber > (SELECT EventRowNumber FROM {KeyTableName} WHERE EventId = @BaseEventId)
                  ORDER BY EventRowNumber ASC LIMIT @Count;
{/ReadAllStreamsChunksProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadStreamChunkByStreamIdProcedure}
    SELECT * FROM {KeyTableName} WHERE
                  {KeyTableName}.StreamId = @StreamId
                                AND EventRowNumber > (SELECT EventRowNumber FROM {KeyTableName} WHERE EventId = @BaseEventId)
                  ORDER BY EventRowNumber ASC LIMIT @Count;
{/ReadStreamChunkByStreamIdProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}