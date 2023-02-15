using System;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Macros;

namespace Meadow.MySql.Scaffolding.Macros
{

    public class EventSourcingMySqlMacro : MacroBase
    {
        public override string Name { get; } = "EventStream";


        private readonly IDbTypeNameMapper _typeNameMap = new MySqlDbTypeNameMapper();

        public override string GenerateCode(params string[] arguments)
        {

            var entityType = GrabTypeArgument(arguments, 0);

            var streamIdLeaf = TypeIdentity.FindIdentityLeaf(entityType);
            
            var convention = new NameConvention(entityType);
            
            
            
            var streamIdType = _typeNameMap[streamIdLeaf.Type];

            var eventIdType = GetEventIdType(arguments);

            var typeNameSize = ReadNumberArgument(arguments, "typenamelength-", 128);
            
            var valueSize = ReadNumberArgument(arguments, "eventdatalength-", 256);

            var dbGenerated = eventIdType == "bigint" ? " AUTO_INCREMENT" : "";
            
            var code = _macroContent;

            code = code.Replace("{ENTITY}", convention.EntityName);
            code = code.Replace("{TABLE_NAME}", convention.EventStreamTableName);
            code = code.Replace("{EV_TYPE}", eventIdType);
            code = code.Replace("{ST_TYPE}", streamIdType);
            code = code.Replace("{TYPE_SIZE}", typeNameSize);
            code = code.Replace("{VALUE_SIZE}", valueSize);
            code = code.Replace("{DB_GENERATED}", dbGenerated);
            code = code.Replace("{INSERT_EVENT_PROCEDURE_NAME}", convention.InsertEvent);
            code = code.Replace("{READ_ALL_STREAMS_PROCEDURE_NAME}", convention.ReadAllStreams);
            code = code.Replace("{READ_STREAM_BY_STREAMID_PROCEDURE_NAME}", convention.ReadStreamByStreamId);
            code = code.Replace("{READ_ALL_STREAMS_CHUNKS_PROCEDURE_NAME}", convention.ReadAllStreamsChunks);
            code = code.Replace("{READ_STREAM_CHUNK_BY_STREAMID_PROCEDURE_NAME}", convention.ReadStreamChunkByStreamId);
            

            return code;
        }

        private string GetEventIdType(string[] args)
        {
            if (args.Any(a => a.ToLower() == "guid"))
            {
                return _typeNameMap[typeof(Guid)];
            }

            return "bigint";
        }
        
        private string ReadNumberArgument(string[] args,string startTag,int def)
        {

            foreach (var arg in args)
            {
                if (!string.IsNullOrWhiteSpace(arg))
                {
                    if (arg.ToLower().StartsWith(startTag))
                    {
                        var stringLength = arg.Substring(startTag.Length, arg.Length - startTag.Length);

                        if (int.TryParse(stringLength, out _))
                        {
                            return stringLength;
                        }
                    }
                }
            }

            return def.ToString();
        }
        
        private readonly string _macroContent =
            @"-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE {TABLE_NAME} (
    EventId {EV_TYPE} PRIMARY KEY{DB_GENERATED},
    StreamId {ST_TYPE},
    TypeName varchar({TYPE_SIZE}),
    SerializedValue varchar({VALUE_SIZE})
);
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {INSERT_EVENT_PROCEDURE_NAME}(IN StreamId {ST_TYPE},
                                   IN TypeName varchar({TYPE_SIZE}),
                                   IN SerializedValue varchar({VALUE_SIZE})) 
BEGIN

    INSERT INTO {TABLE_NAME} (StreamId, TypeName, SerializedValue) 
        VALUES (StreamId,TypeName,SerializedValue);
    
    SELECT * FROM {TABLE_NAME} WHERE EventId=LAST_INSERT_ID();
    
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {READ_ALL_STREAMS_PROCEDURE_NAME}()
BEGIN

    SELECT * FROM {TABLE_NAME};

END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {READ_STREAM_BY_STREAMID_PROCEDURE_NAME}(IN StreamId {ST_TYPE})
BEGIN

    SELECT * FROM {TABLE_NAME} WHERE {TABLE_NAME}.StreamId = StreamId;

END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {READ_ALL_STREAMS_CHUNKS_PROCEDURE_NAME}(IN StreamId {ST_TYPE},
                                         IN BaseEventId {EV_TYPE},
                                         IN Count bigint)
BEGIN

    SELECT * FROM {TABLE_NAME} WHERE
            {TABLE_NAME}.StreamId = StreamId
                                AND EventId > BaseEventId
    LIMIT Count;

END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {READ_STREAM_CHUNK_BY_STREAMID_PROCEDURE_NAME}(IN BaseEventId {EV_TYPE},
                                        IN Count bigint)
BEGIN

    SELECT * FROM {TABLE_NAME} WHERE
            {TABLE_NAME}.StreamId = StreamId
                                AND EventId > BaseEventId
    LIMIT Count;

END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
";
    }
}