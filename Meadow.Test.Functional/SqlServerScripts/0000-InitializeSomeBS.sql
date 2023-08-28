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
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersons AS
    SELECT * FROM Persons ;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersonsFullTree AS
    SELECT * FROM PersonsFullTree ;
GO
-- ---------------------------------------------------------------------------------------------------------------------

CREATE VIEW PersonsFullTree AS
SELECT Persons.Id        'Persons_Id',
       Persons.Name        'Name',
       Persons.Surname        'Surname',
       Persons.Age        'Age',
       Persons.JobId        'JobId',
       Jobs.Id        'Persons_Jobs_Id',
       Jobs.Title        'Title',
       Jobs.IncomeInRials        'IncomeInRials',
       Jobs.JobDescription        'JobDescription',
       Addresses.City        'City',
       Addresses.Street        'Street',
       Addresses.AddressName        'AddressName',
       Addresses.Block        'Block',
       Addresses.Plate        'Plate',
       Addresses.Id        'Persons_Addresses_Id',
       Addresses.PersonId        'PersonId'
FROM   Persons
           INNER JOIN Jobs ON Persons.JobId = Jobs.Id
           INNER JOIN Addresses ON Addresses.PersonId = Persons.Id;


