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

