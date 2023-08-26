
drop function if exists "spRemoveExpiredFilterResults";
drop function if exists "spPerformPersonsFilterIfNeeded"(par_FilterHash text, par_ExpirationTimeStamp integer, par_FilterExpression text);
drop function if exists "spPerformPersonsFilterIfNeeded";
drop function if exists "spReadPersonsChunk";

drop function if exists "spInsertPerson"(par_Name TEXT, par_Surname TEXT, par_Age INT, par_JobId INT);
drop function if exists "spGetAllPersons";
drop function if exists "spGetAllPersonsFullTree";
drop function if exists "spGetPersonById"(par_Id bigint);

drop table if exists "Persons";


-- Table Example.Postgre.Models.Person
-- ---------------------------------------------------------------------------------------------------------------------
-- TableCodeGenerator
-- ---------------------------------------------------------------------------------------------------------------------

create table "Persons"("Id" SERIAL,
    "Name" TEXT,
    "Surname" TEXT,
    "Age" INT,
    "JobId" INT,
    PRIMARY KEY ("Id")
);
------------------------------------------------------------------------------------------------------------------------
-- SPLIT
------------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- </Table>
-- ---------------------------------------------------------------------------------------------------------------------

-- Insert Example.Postgre.Models.Person
-- ---------------------------------------------------------------------------------------------------------------------
-- InsertCodeGenerator
-- ---------------------------------------------------------------------------------------------------------------------
create function "spInsertPerson"("par_Name" TEXT,"par_Surname" TEXT,"par_Age" INT,"par_JobId" INT) returns setof "Persons" as $$
        begin
        return query
            insert into "Persons" ("Name","Surname","Age","JobId") 
            values ("par_Name","par_Surname","par_Age","par_JobId")
        returning * ;
        end;
        $$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- </Insert>
-- ---------------------------------------------------------------------------------------------------------------------

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
    delete from "FilterResults" where "FilterResults"."ExpirationTimeStamp" >= "par_ExpirationTimeStamp";
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function "spPerformPersonsFilterIfNeeded" 
                ("par_FilterHash" TEXT,
                "par_ExpirationTimeStamp" INT,
                "par_FilterExpression" TEXT) 
    returns setof "FilterResults" as $$
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
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
create function "spReadPersonsChunk"
                ("par_Offset" INT,
                 "par_Size" INT,
                 "par_FilterHash" TEXT) returns setof "Persons" as $$
begin
    return query select "Persons".* from "Persons" 
        inner join (select * from "FilterResults" where "FilterResults"."FilterHash"="par_FilterHash") "FR"
        on "Persons"."Id" = "FR"."ResultId";
end;
$$ language plpgsql;
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- </Filtering>
-- ---------------------------------------------------------------------------------------------------------------------



select * from "spInsertPerson"('Mani','Moayedi',37,1);
select * from "spInsertPerson"('Mona','Moayedi',42,2);
select * from "spInsertPerson"('Mina','Haddadi',56,3);
select * from "spInsertPerson"('Farshid','Moayedi',63,4);
select * from "spInsertPerson"('Farimehr','Ayerian',21,5);


delete from "FilterResults" where 1=1;

select * from "spPerformPersonsFilterIfNeeded"('mash5',123,'"Age" > 50 AND "Name" = ''Mina''');
select * from "spReadPersonsChunk"(0,20,'mash5');
