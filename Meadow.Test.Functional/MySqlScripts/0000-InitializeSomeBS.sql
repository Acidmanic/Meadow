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
CREATE PROCEDURE spReadAllPersons()
BEGIN
    SELECT * FROM Persons ;
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersonsFullTree()
BEGIN
    SELECT * FROM PersonsFullTree ;
END;
-- ---------------------------------------------------------------------------------------------------------------------


-- SPLIT