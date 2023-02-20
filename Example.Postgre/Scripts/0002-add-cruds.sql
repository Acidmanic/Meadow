--     
create or replace function "spGetAllPersons"() returns setof "Persons" as $$
begin 
    return QUERY
    select * from "Persons";
end;
$$ language plpgsql;
--SPLIT

create or replace function "spGetPersonById"("par_Id" bigint) returns setof "Persons" as $$ 
begin
    return query
    select * from "Persons" where "Id" = "par_Id";
end;
$$ language plpgsql ;
--SPLIT

create type "PersonsFullTree" as (Persons_Id INT,"Name" TEXT,"Surname" TEXT,"Age" INT,"JobId" INT,
                                    "Jobs_Id" INT,"Title" TEXT,"JobDescription" TEXT,"IncomeInRials" INT);

--SPLIT

create or replace function "spGetAllPersonsFullTree"() returns SETOF "PersonsFullTree"  as $$
begin
    return query
    select
        "Persons"."Id" as "Persons_Id", "Persons"."Name", "Persons"."Surname", "Persons"."Age",
        "Persons"."JobId","Jobs"."Id" as "Jobs_Id","Jobs"."Title","Jobs"."JobDescription",
        "Jobs"."IncomeInRials"
    from "Persons" join "Jobs"  on "Jobs"."Id" = "Persons"."JobId";
end;
$$ language plpgsql;


 
create or replace function "spInsertPerson"(
    "par_Name" varchar(100),
    "par_Surname" varchar(100),
    "par_Age" int,
    "par_JobId" int) returns setof "Persons" as $$
begin
    return query
    insert into "Persons" ("Name", "Surname", "Age", "JobId") 
    values ("par_Name","par_Surname","par_Age","par_JobId")
    returning * ;
end;
$$ language plpgsql;



insert into "Persons" ("Name", "Surname", "Age", "JobId") 
    values ('Mona','Moayedi',12,2); 


 select * from "Persons";


create or replace function "spDeleteAsISay"( "par_Id" bigint) returns setof bool as $$
begin
    
        delete from "Persons" where "Id" = "par_Id";
        return query
        select true as "Success";

end;
$$ language plpgsql;


create or replace function "spDeleteAsISay"( "par_Id" bigint) returns TABLE("Success" bool) as $$
DECLARE
    count int := 0;
    change int :=0;
BEGIN
    count := (select Count(*) from "Persons");
    delete from "Persons" where "Id" = "par_Id";
    change := (select Count(*) from "Persons");
    if change < count THEN
        return query select true as "Success";
    else
        return query select false as "Success";
    end if;
end;
$$ language plpgsql;


select * from "spDeleteAsISay"(8);


create or replace function "spUpdateAsISay"(
    "par_Id" bigint,
    "par_Name" varchar(100),
    "par_Surname" varchar(100),
    "par_Age" int,
    "par_JobId" int) returns setof "Persons" as $$
begin
    return query
        update "Persons" set "Name" = "par_Name", "Surname" = "par_Surname" 
            , "Age" =  "par_Age", "JobId" = "par_JobId"
            where "Id" = "par_Id"
            returning * ;
end;
$$ language plpgsql;


select * from "spUpdateAsISay"(7,'Acidmanic','Mouayedi',37,2);


select * from "Persons";


create or replace function "spSaveAsISay"(
    "par_Id" bigint,
    "par_Name" varchar(100),
    "par_Surname" varchar(100),
    "par_Age" int,
    "par_JobId" int) returns setof "Persons" as $$
declare 
    updateCount int := 0;
    
begin
    
    updateCount := (select count(*) from "Persons" where "Id"="par_Id");
    
    if(updateCount>0) then
        return query
            update "Persons" set "Name" = "par_Name", "Surname" = "par_Surname"
                , "Age" =  "par_Age", "JobId" = "par_JobId"
                where "Id" = "par_Id"
                returning * ;
    else
        return query
            insert into "Persons" ("Name", "Surname", "Age", "JobId")
                values ("par_Name","par_Surname","par_Age","par_JobId")
                returning * ;
    end if;
end;
$$ language plpgsql;



select * from "Persons";

select * from "spSaveAsISay"(11,'pashang','Mouayedi',37,2);


do $$
    DECLARE
        counter    INTEGER := 0;
    BEGIN
        
        select * from "Persons";
        select "spDeleteAsISay"(9);
    END;
$$




DO $$
DECLARE
    count int := 0;
    change int :=0;
BEGIN
    count := (select Count(*) from "Persons");
    delete from "Persons" where "Id" = 8;
    change := (select Count(*) from "Persons");
    if change < count THEN 
            return query select true as Success;
        else
            return query select false as Success;
    end if;
END $$;