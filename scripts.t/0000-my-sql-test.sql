-- EventStream Example.Macros.MySql.Models.IPersonEvent
-- ---------------------------------------------------------------------------------------------------------------------
-- EventStreamSqlScriptGenerator
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE PersonsEventStream (
    EventId BIGINT(16) PRIMARY KEY AUTO_INCREMENT,
    StreamId BIGINT(16),
    TypeName varchar(256),
    SerializedValue varchar(1024));
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spInsertPersonEvent(
                                   IN StreamId BIGINT(16),
                                   IN TypeName varchar(256),
                                   IN SerializedValue varchar(1024)) 
BEGIN

    INSERT INTO PersonsEventStream (StreamId, TypeName, SerializedValue) 
        VALUES (StreamId,TypeName,SerializedValue);
    
    SELECT * FROM PersonsEventStream WHERE EventId=LAST_INSERT_ID();
    
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersonStreams()
BEGIN

    SELECT * FROM PersonsEventStream;

END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPersonStreamByStreamId(IN StreamId BIGINT(16))
BEGIN

    SELECT * FROM PersonsEventStream WHERE PersonsEventStream.StreamId = StreamId;

END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersonStreamsChunk(
                                         IN BaseEventId BIGINT(16),
                                         IN Count bigint)
BEGIN

    SELECT * FROM PersonsEventStream WHERE EventId > BaseEventId

    LIMIT Count;

END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPersonStreamChunkByStreamId(
                                        IN StreamId BIGINT(16),
                                        IN BaseEventId BIGINT(16),
                                        IN Count bigint)
BEGIN

    SELECT * FROM PersonsEventStream WHERE
                  PersonsEventStream.StreamId = StreamId
                                AND EventId > BaseEventId
    LIMIT Count;

END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- </EventStream>
-- ---------------------------------------------------------------------------------------------------------------------


call spInsertPersonEvent (123,'System.Mani','some-jibberish');
call spInsertPersonEvent (123,'System.Mani','more-jibberish');
call spInsertPersonEvent (456,'System.Mona','some-more-jibber');
call spInsertPersonEvent (456,'System.Mona','some-more-jabber');

call spReadAllPersonStreams();

call spReadPersonStreamByStreamId (123);
call spReadPersonStreamByStreamId (456);

call spReadAllPersonStreams();

call spReadAllPersonStreamsChunk (0,100);
call spReadAllPersonStreamsChunk (2,100);
call spReadAllPersonStreamsChunk (0,1);
call spReadAllPersonStreamsChunk (2,1);
