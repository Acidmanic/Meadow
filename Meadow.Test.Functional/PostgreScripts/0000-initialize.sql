-- Crud Meadow.Test.Functional.Models.Address

-- Crud Meadow.Test.Functional.Models.Job

-- Crud Meadow.Test.Functional.Models.Person

-- Filtering Meadow.Test.Functional.Models.Person

-- SPLIT 

-- {{Table Meadow.Test.Functional.Models.Address}}
-- {{Insert Meadow.Test.Functional.Models.Address}}
-- {{Table Meadow.Test.Functional.Models.Job}}
-- {{Insert Meadow.Test.Functional.Models.Job}}
-- {{Table Meadow.Test.Functional.Models.Person}}
-- {{Insert Meadow.Test.Functional.Models.Person}}
-- {{FullTreeView Meadow.Test.Functional.Models.Person}}
-- {{Filtering Meadow.Test.Functional.Models.Person}}
-- ---------------------------------------------------------------------------------------------------------------------

create or replace function "spReadAllPersons"() returns setof "Persons" as $$
begin
    return QUERY
        select * from "Persons";
end;
$$ language plpgsql;
--SPLIT
create or replace function "spReadAllPersonsFullTree"() returns setof "PersonsFullTree" as $$
begin
    return QUERY
        select * from "PersonsFullTree";
end;
$$ language plpgsql;
--SPLIT
-- ---------------------------------------------------------------------------------------------------------------------

-- SPLIT

