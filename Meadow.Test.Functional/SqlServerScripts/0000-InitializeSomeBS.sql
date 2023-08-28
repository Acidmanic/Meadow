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

-- ---------------------------------------------------------------------------------------------------------------------
CREATE VIEW PersonsFullTree AS
SELECT Persons.Id 'Persons_Id',
       Persons.Name 'Name',
       Persons.Age 'Age',
       Persons.JobId 'JobId',
       Persons.Surname 'Surname',
       Jobs.Id 'Persons_Jobs_Id',
       Jobs.IncomeInRials 'IncomeInRials',
       Jobs.JobDescription 'JobDescription',
       Jobs.Title 'Title',
       Addresses.PersonId 'PersonId',
       Addresses.Id 'Persons_Addresses_Id',
       Addresses.Plate 'Plate',
       Addresses.AddressName 'AddressName',
       Addresses.Block 'Block',
       Addresses.City 'City',
       Addresses.Street 'Street'
from Persons
         inner join Jobs on Persons.JobId = Jobs.Id
         inner join Addresses on Persons.Id = Addresses.PersonId;
GO

-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersons AS
    SELECT * FROM Persons ;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersonsFullTree AS
    SELECT * FROM PersonsFullTree ;
GO
-- ---------------------------------------------------------------------------------------------------------------------
