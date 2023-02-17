using System;
using System.Collections.Generic;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class EventStreamSqlScriptGenerator<TEvent> : EventStreamSqlScriptGenerator
    {
        public EventStreamSqlScriptGenerator() : base(typeof(TEvent))
        {
        }
    }

    public class EventStreamSqlScriptGenerator : ByTemplateSqlGeneratorBase
    {
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyEventIdType = GenerateKey();
        private readonly string _keyAutogenerated = GenerateKey();
        private readonly string _keyStreamIdType = GenerateKey();
        private readonly string _keyTypeNameType = GenerateKey();
        private readonly string _keySerializedValueType = GenerateKey();

        private readonly string _keyInsertProcedureName = GenerateKey();
        private readonly string _keyEventIdInsertParameter = GenerateKey();
        private readonly string _keyEventIdInsertValueAndColumn = GenerateKey();
        private readonly string _keyInsertSelectStatement = GenerateKey();
        private readonly string _keyReadAllStreamsProcedureName = GenerateKey();
        private readonly string _keyReadStreamByStreamIdProcedureName = GenerateKey();
        private readonly string _keyReadAllStreamsChunksProcedureName = GenerateKey();
        private readonly string _keyReadStreamChunkByStreamIdProcedureName = GenerateKey();

        protected ProcessedType ProcessedType { get; }

        public EventStreamSqlScriptGenerator(Type type) : base(new MySqlDbTypeNameMapper())
        {
            ProcessedType = Process(type);
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.EventStreamTableName);

            replacementList.Add(_keyEventIdType, ProcessedType.EventIdTypeName);

            replacementList.Add(_keyAutogenerated, ProcessedType.IsEventIdAutogenerated ? "AUTO_INCREMENT" : "");

            replacementList.Add(_keyStreamIdType, ProcessedType.StreamIdTypeName);

            replacementList.Add(_keyTypeNameType, ProcessedType.EventStreamTypeNameDatabaseType);

            replacementList.Add(_keySerializedValueType, ProcessedType.EventStreamSerializedValueDatabaseType);

            replacementList.Add(_keyEventIdInsertParameter,
                ProcessedType.IsEventIdAutogenerated ? "" : $"IN EventId {ProcessedType.EventIdTypeName},");

            replacementList.Add(_keyInsertProcedureName, ProcessedType.NameConvention.InsertEvent);

            replacementList.Add(_keyEventIdInsertValueAndColumn,
                ProcessedType.IsEventIdAutogenerated ? "" : ", EventId");

            var insertSelectStatement = ProcessedType.IsEventIdAutogenerated?
                    $"SELECT * FROM {ProcessedType.NameConvention.EventStreamTableName} WHERE EventId=LAST_INSERT_ID();":
                    "SELECT EventId 'EventId',StreamId 'StreamId', TypeName 'TypeName', SerializedValue 'SerializedValue';";
            replacementList.Add(_keyInsertSelectStatement,insertSelectStatement);
            
            
            replacementList.Add(_keyReadAllStreamsProcedureName,ProcessedType.NameConvention.ReadAllStreams);
            
            replacementList.Add(_keyReadStreamByStreamIdProcedureName,ProcessedType.NameConvention.ReadStreamByStreamId);
            
            replacementList.Add(_keyReadAllStreamsChunksProcedureName,ProcessedType.NameConvention.ReadAllStreamsChunks);
            
            replacementList.Add(_keyReadStreamChunkByStreamIdProcedureName,ProcessedType.NameConvention.ReadStreamChunkByStreamId);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE {_keyTableName} (
    EventId {_keyEventIdType} PRIMARY KEY {_keyAutogenerated},
    StreamId {_keyStreamIdType},
    TypeName {_keyTypeNameType},
    SerializedValue {_keySerializedValueType});
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyInsertProcedureName}({_keyEventIdInsertParameter}
                                   IN StreamId {_keyStreamIdType},
                                   IN TypeName {_keyTypeNameType},
                                   IN SerializedValue {_keySerializedValueType}) 
BEGIN

    INSERT INTO {_keyTableName} (StreamId, TypeName, SerializedValue{_keyEventIdInsertValueAndColumn}) 
        VALUES (StreamId,TypeName,SerializedValue{_keyEventIdInsertValueAndColumn});
    
    {_keyInsertSelectStatement}
    
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadAllStreamsProcedureName}()
BEGIN

    SELECT * FROM {_keyTableName};

END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadStreamByStreamIdProcedureName}(IN StreamId {_keyStreamIdType})
BEGIN

    SELECT * FROM {_keyTableName} WHERE {_keyTableName}.StreamId = StreamId;

END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadAllStreamsChunksProcedureName}(
                                         IN BaseEventId {_keyEventIdType},
                                         IN Count bigint)
BEGIN

    SELECT * FROM {_keyTableName} WHERE EventId > BaseEventId

    LIMIT Count;

END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadStreamChunkByStreamIdProcedureName}(
                                        IN StreamId {_keyStreamIdType},
                                        IN BaseEventId {_keyEventIdType},
                                        IN Count bigint)
BEGIN

    SELECT * FROM {_keyTableName} WHERE
                  {_keyTableName}.StreamId = StreamId
                                AND EventId > BaseEventId
    LIMIT Count;

END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}