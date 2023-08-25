
-- am tp ../Example.Postgre sd ../scripts.t

-- EventStream Example.Postgre.Models.IPersonEvent
-- ---------------------------------------------------------------------------------------------------------------------
-- EventStreamCodeGenerator
-- ---------------------------------------------------------------------------------------------------------------------

create table "PersonsEventStream"("EventId" SERIAL,
        "StreamId" INT,
        "TypeName" TEXT,
        "SerializedValue" TEXT,
        PRIMARY KEY ("EventId")
);
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function "spInsertPersonEvent"(
        "par_StreamId" INT,
        "par_TypeName" TEXT,
        "par_SerializedValue" TEXT) returns setof "PersonsEventStream" as $$
        begin
            return query
                insert into "PersonsEventStream" (
                    "StreamId",
                    "TypeName",
                    "SerializedValue")
                values (
                    "par_StreamId",
                    "par_TypeName",
                    "par_SerializedValue")
            returning * ;
        end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function "spReadAllPersonStreams"() returns setof "PersonsEventStream" as $$
    begin
        return query
            select * from "PersonsEventStream";
    end;
$$ language plpgsql ;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function "spReadPersonStreamByStreamId"("par_StreamId" INT) returns setof "PersonsEventStream" as $$
    begin
        return query
            select * from "PersonsEventStream" where "StreamId" = "par_StreamId";
    end;
$$ language plpgsql ;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------

create or replace function "spReadAllPersonStreamsChunk"(
    "par_BaseEventId" INT,
    "par_Count" INT ) returns setof "PersonsEventStream" as $$
    begin
        return query
            select * from "PersonsEventStream" 
                where "EventId" > "par_BaseEventId"
                limit "par_Count";
    end;
$$ language plpgsql ;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function "spReadPersonStreamChunkByStreamId"(
    "par_StreamId" INT,
    "par_BaseEventId" INT,
    "par_Count" INT ) returns setof "PersonsEventStream" as $$
    begin
    return query
        select * from "PersonsEventStream" 
            where "StreamId" = "par_StreamId"  
            and "EventId" > "par_BaseEventId"
            limit "par_Count";
    end;
$$ language plpgsql ;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------

-- ---------------------------------------------------------------------------------------------------------------------
-- </EventStream>
-- ---------------------------------------------------------------------------------------------------------------------




select * from "spInsertPersonEvent" (123,'System.Mani','some-jibberish');
select * from  "spInsertPersonEvent" (123,'System.Mani','more-jibberish');
select * from  "spInsertPersonEvent" (456,'System.Mona','some-more-jibber');
select * from  "spInsertPersonEvent" (456,'System.Mona','some-more-jabber');

select * from  "spReadAllPersonStreams"();

select * from  "spReadPersonStreamByStreamId" (123);
select * from  "spReadPersonStreamByStreamId" (456);

select * from  "spReadAllPersonStreams"();

select * from  "spReadAllPersonStreamsChunk" (0,100);
select * from  "spReadAllPersonStreamsChunk" (2,100);
select * from  "spReadAllPersonStreamsChunk" (0,1);
select * from  "spReadAllPersonStreamsChunk" (2,1);



-- create or replace function "spMooz"() returns setof "text" as $$


select * from "spMooz"();