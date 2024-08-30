using System;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.EventStreamScript)]
public class EventStreamSnippet:ISnippet
{
    public SnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public string KeyTableName => Toolbox.ProcessedType.NameConvention.EventStreamTableName;
    public string KeyEventIdType => Toolbox.ProcessedType.EventIdTypeName;
    public string KeyStreamIdType => Toolbox.ProcessedType.StreamIdTypeName;
    public string KeyTypeNameType => Toolbox.ProcessedType.EventStreamTypeNameDatabaseType;
    public string KeyAssemblyNameType => Toolbox.ProcessedType.EventStreamAssemblyNameDatabaseType;
    public string KeySerializedValueType => Toolbox.ProcessedType.EventStreamSerializedValueDatabaseType;
    public string KeyInsertProcedureName => Toolbox.ProcessedType.NameConvention.InsertEvent;
    public string KeyEventIdDefinition => Toolbox.EventIdDefinitionPhrase(",");
    public string KeyEventIdInsertParameter => Toolbox.EventIdProcedureParameterPhrase(",");
    public string KeyEventIdInsertColumn => Toolbox.ProcessedType.IsEventIdAutogenerated ? "" : ", EventId";
    public string KeyEventIdInsertValue => Toolbox.ProcessedType.IsEventIdAutogenerated ? "" : $", {Toolbox.SqlTranslator.ProcedureBodyParameterNamePrefix}EventId";
    public string KeyReadAllStreamsProcedureName => Toolbox.ProcessedType.NameConvention.ReadAllStreams;
    public string KeyReadStreamByStreamIdProcedureName => Toolbox.ProcessedType.NameConvention.ReadStreamByStreamId;
    public string KeyReadAllStreamsChunksProcedureName => Toolbox.ProcessedType.NameConvention.ReadAllStreamsChunks;
    public string KeyReadStreamChunkByStreamIdProcedureName => Toolbox.ProcessedType.NameConvention.ReadStreamChunkByStreamId;
    
    
    public string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE {KeyTableName} (
    {KeyEventIdDefinition}
    StreamId {KeyStreamIdType},
    TypeName {KeyTypeNameType},
    AssemblyName {KeyAssemblyNameType},
    SerializedValue {KeySerializedValueType});
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {KeyInsertProcedureName}({KeyEventIdInsertParameter}
                                   @StreamId {KeyStreamIdType},
                                   @TypeName {KeyTypeNameType},
                                   @AssemblyName {KeyAssemblyNameType},
                                   @SerializedValue {KeySerializedValueType}) AS

    INSERT INTO {KeyTableName} (EventId,StreamId, TypeName,AssemblyName, SerializedValue) 
        VALUES (@EventId,@StreamId,@TypeName,@AssemblyName,@SerializedValue);
    
    SELECT * FROM {KeyTableName} WHERE ROWID=LAST_INSERT_ROWID(); 
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {KeyReadAllStreamsProcedureName} AS
    SELECT * FROM {KeyTableName};
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {KeyReadStreamByStreamIdProcedureName}(@StreamId {KeyStreamIdType}) AS
    SELECT * FROM {KeyTableName} WHERE {KeyTableName}.StreamId = @StreamId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {KeyReadAllStreamsChunksProcedureName}(
                                         @BaseEventId {KeyEventIdType},
                                         @Count INTEGER) AS
    SELECT * FROM {KeyTableName} WHERE EventId > @BaseEventId LIMIT @Count;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {KeyReadStreamChunkByStreamIdProcedureName}(
                                        @StreamId {KeyStreamIdType},
                                        @BaseEventId {KeyEventIdType},
                                        @Count INTEGER) AS 
    SELECT * FROM {KeyTableName} WHERE
                  {KeyTableName}.StreamId = @StreamId
                                AND EventId > @BaseEventId LIMIT @Count;
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}