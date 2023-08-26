
-- am tp ../Example.Postgre sd ../scripts.t

-- Filtering Example.Postgre.Models.Person
-- ---------------------------------------------------------------------------------------------------------------------
-- TableCodeGenerator
-- ---------------------------------------------------------------------------------------------------------------------

create table if not exists "FilterResults"("Id" SERIAL,
    "FilterHash" TEXT,
    "ResultId" INT,
    "ExpirationTimeStamp" INT,
    PRIMARY KEY ("Id")
);
------------------------------------------------------------------------------------------------------------------------
-- SPLIT
------------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- FilteringProceduresGenerator
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
create or replace function "spRemoveExpiredFilterResults"("par_ExpirationTimeStamp" INT) 
    returns void as $$ 
begin
    delete from "FilterResults" where "FilterResults".ExpirationTimeStamp >= "par_ExpirationTimeStamp";
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- </Filtering>
-- ---------------------------------------------------------------------------------------------------------------------

delete from "FilterResults" where 1=1;
drop function if exists "performFilter";
create function "performFilter" (
    "par_FilterHash" text,
    "par_ExpirationTimeStamp" INT,
    "par_FilterExpression" TEXT) returns setof "FilterResults" as $$
    declare sql text = '';
begin
    if "par_FilterExpression" is null or "par_FilterExpression" ='' then
        "par_FilterExpression" = 'true';
    end if;
    sql = CONCAT('insert into "FilterResults" ("FilterHash", "ResultId", "ExpirationTimeStamp") 
            select ''', "par_FilterHash",''',"Persons"."Id", ', "par_ExpirationTimeStamp",' from "Persons"
            where ',"par_FilterExpression",';');
    if not exists(select 1 from "FilterResults" where "FilterHash" = "par_FilterHash") then
        execute sql;
    end if;
    return query select * from "FilterResults";
end;
$$ language plpgsql;
    
select * from "performFilter"('mash5',123,'');

drop function if EXISTS "chunk";
create function "chunk"( "par_Offset" INT,
                         "par_Size" INT,
                         "par_FilterHash" TEXT) returns setof "Persons" as $$
begin
    return query select "Persons".* from "Persons" 
        inner join (select * from "FilterResults" where "FilterResults"."FilterHash"="par_FilterHash") "FR"
        on "Persons"."Id" = "FR"."ResultId";
end;
$$ language plpgsql;

select * from "chunk"(0,20,'mash5');

