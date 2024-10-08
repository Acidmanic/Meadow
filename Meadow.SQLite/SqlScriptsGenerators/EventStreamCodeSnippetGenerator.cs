using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.EventStreamScript)]
    public class EventStreamCodeSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public EventStreamCodeSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution()
            {
                SqlTranslator = new SqLiteTranslator(construction.MeadowConfiguration),
                TypeNameMapper = new SqLiteTypeNameMapper()
            })
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            base.DeclareUnSupportedFeatures(declaration);

            declaration.NotSupportedRepetitionHandling();
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyEventIdType = GenerateKey();
        private readonly string _keyAutogenerated = GenerateKey();
        private readonly string _keyStreamIdType = GenerateKey();
        private readonly string _keyTypeNameType = GenerateKey();
        private readonly string _keySerializedValueType = GenerateKey();

        private readonly string _keyEventIdInsertParameter = GenerateKey();
        private readonly string _keyInsertProcedureName = GenerateKey();
        private readonly string _keyEventIdInsertColumn = GenerateKey();
        private readonly string _keyEventIdInsertValue = GenerateKey();
        private readonly string _keyReadAllStreamsProcedureName = GenerateKey();
        private readonly string _keyReadStreamByStreamIdProcedureName = GenerateKey();
        private readonly string _keyReadAllStreamsChunksProcedureName = GenerateKey();
        private readonly string _keyReadStreamChunkByStreamIdProcedureName = GenerateKey();


        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.EventStreamTableName);
            replacementList.Add(_keyEventIdType, ProcessedType.EventIdTypeName);
            replacementList.Add(_keyAutogenerated, ProcessedType.IsEventIdAutogenerated ? "AUTOINCREMENT" : "");
            replacementList.Add(_keyStreamIdType, ProcessedType.StreamIdTypeName);
            replacementList.Add(_keyTypeNameType, ProcessedType.EventStreamTypeNameDatabaseType);
            replacementList.Add(_keySerializedValueType, ProcessedType.EventStreamSerializedValueDatabaseType);

            replacementList.Add(_keyEventIdInsertParameter,
                ProcessedType.IsEventIdAutogenerated ? "" : $"@EventId {ProcessedType.EventIdTypeName},");

            replacementList.Add(_keyInsertProcedureName, ProcessedType.NameConvention.InsertEventProcedure);

            replacementList.Add(_keyEventIdInsertColumn,
                ProcessedType.IsEventIdAutogenerated ? "" : ", EventId");
            replacementList.Add(_keyEventIdInsertValue,
                ProcessedType.IsEventIdAutogenerated ? "" : ", @EventId");

            replacementList.Add(_keyReadAllStreamsProcedureName, ProcessedType.NameConvention.ReadAllStreams);
            replacementList.Add(_keyReadStreamByStreamIdProcedureName,
                ProcessedType.NameConvention.ReadStreamByStreamId);
            replacementList.Add(_keyReadAllStreamsChunksProcedureName,
                ProcessedType.NameConvention.ReadAllStreamsChunks);
            replacementList.Add(_keyReadStreamChunkByStreamIdProcedureName,
                ProcessedType.NameConvention.ReadStreamChunkByStreamId);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE {_keyTableName} (
    EventId {_keyEventIdType} NOT NULL PRIMARY KEY {_keyAutogenerated},
    StreamId {_keyStreamIdType},
    TypeName {_keyTypeNameType},
    SerializedValue {_keySerializedValueType});
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyInsertProcedureName}({_keyEventIdInsertParameter}
                                   @StreamId {_keyStreamIdType},
                                   @TypeName {_keyTypeNameType},
                                   @SerializedValue {_keySerializedValueType}) AS

    INSERT INTO {_keyTableName} (StreamId, TypeName, SerializedValue{_keyEventIdInsertColumn}) 
        VALUES (StreamId,TypeName,SerializedValue{_keyEventIdInsertValue});
    
    SELECT * FROM {_keyTableName} WHERE ROWID=LAST_INSERT_ROWID(); 
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadAllStreamsProcedureName} AS
    SELECT * FROM {_keyTableName};
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadStreamByStreamIdProcedureName}(@StreamId {_keyStreamIdType}) AS
    SELECT * FROM {_keyTableName} WHERE {_keyTableName}.StreamId = @StreamId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadAllStreamsChunksProcedureName}(
                                         @BaseEventId {_keyEventIdType},
                                         @Count INTEGER) AS
    SELECT * FROM {_keyTableName} WHERE EventId > @BaseEventId LIMIT @Count;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyReadStreamChunkByStreamIdProcedureName}(
                                        @StreamId {_keyStreamIdType},
                                        @BaseEventId {_keyEventIdType},
                                        @Count INTEGER) AS 
    SELECT * FROM {_keyTableName} WHERE
                  {_keyTableName}.StreamId = @StreamId
                                AND EventId > @BaseEventId LIMIT @Count;
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}