-- ---------------------------------------------------------------------------------------------------------------------
-- EventStream Example.SqLite.Models.IPersonEvent
-- ---------------------------------------------------------------------------------------------------------------------
-- EventStreamCodeGenerator
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE PersonsEventStream (
    EventId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    StreamId INTEGER,
    TypeName TEXT,
    SerializedValue TEXT);
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spInsertPersonEvent(
                                   @StreamId INTEGER,
                                   @TypeName TEXT,
                                   @SerializedValue TEXT) AS

    INSERT INTO PersonsEventStream (StreamId, TypeName, SerializedValue) 
        VALUES (StreamId,TypeName,SerializedValue);
    
    SELECT * FROM PersonsEventStream WHERE ROWID=LAST_INSERT_ROWID(); 
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersonStreams AS
    SELECT * FROM PersonsEventStream;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPersonStreamByStreamId(@StreamId INTEGER) AS
    SELECT * FROM PersonsEventStream WHERE PersonsEventStream.StreamId = @StreamId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersonStreamsChunk(
                                         @BaseEventId INTEGER,
                                         @Count INTEGER) AS
    SELECT * FROM PersonsEventStream WHERE EventId > @BaseEventId LIMIT @Count;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPersonStreamChunkByStreamId(
                                        @StreamId INTEGER,
                                        @BaseEventId INTEGER,
                                        @Count INTEGER) AS 
    SELECT * FROM PersonsEventStream WHERE
                  PersonsEventStream.StreamId = @StreamId
                                AND EventId > @BaseEventId LIMIT @Count;
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- </EventStream>
-- ---------------------------------------------------------------------------------------------------------------------

-- ---------------------------------------------------------------------------------------------------------------------

-- ---------------------------------------------------------------------------------------------------------------------
    
--     insert into Persons (Name, Surname, Age, JobId) VALUES ('Mona','Moayedi',23,12)
-- 
-- 
-- select * from Persons
-- 
-- select 
--     CASE
--     WHEN Count(Id) =0
--         THEN  1
--     ELSE
--          0
--     END as result
-- from Persons;
-- 
-- 
--     if 
--         select 1;
--     else
--         select 0
--     end
-- 
-- 
-- select 1 where EXISTS(select * from Persons where Persons.Id=2)
-- 
-- 
-- UPDATE Persons  SET Name='Acid', Surname='Moayedi', Age=23, JobId=12 WHERE  Persons.Id=12;
-- 
-- INSERT INTO Persons (Name, Surname, Age, JobId) SELECT 'Acid' ,'Moayedi',23,12
--     WHERE NOT EXISTS(SELECT * FROM Persons WHERE Persons.Id=12);
-- 
-- SELECT * FROM Persons WHERE Persons.Id=12 OR ROWID = LAST_INSERT_ROWID() LIMIT 1;
-- 
-- 
-- select * from Persons
