
-- Crud Meadow.Test.Functional.Models.Address

-- Crud Meadow.Test.Functional.Models.Job

-- Crud Meadow.Test.Functional.Models.Person

-- Filtering Meadow.Test.Functional.Models.Person



-- {{Crud Meadow.Test.Functional.Models.Address}}

-- {{Crud Meadow.Test.Functional.Models.Job}}

-- {{Crud Meadow.Test.Functional.Models.Person}}

-- {{Filtering Meadow.Test.Functional.Models.Person}}


CREATE VIEW PersonsFullTree AS
SELECT Persons.Id 'Persons_Id',
       Persons.Name 'Name',
       Persons.Age 'Age',
       Persons.JobId 'JobId',
       Persons.Surname 'Surname',
       Jobs.Id 'Jobs_Id',
       Jobs.IncomeInRials 'IncomeInRials',
       Jobs.JobDescription 'JobDescription',
       Jobs.Title 'Title',
       Addresses.PersonId 'PersonId',
       Addresses.Id 'Addresses_Id',
       Addresses.Plate 'Plate',
       Addresses.AddressName 'AddressName',
       Addresses.Block 'Block',
       Addresses.City 'City',
       Addresses.Street 'Street'
from Persons
         inner join Jobs on Persons.JobId = Jobs.Id
         inner join Addresses on Persons.Id = Addresses.PersonId;
GO


-- SPLIT 

execute spInsertJob 'Mani Job',100,'Mani job Description'  
execute spInsertJob 'Mona Job',100,'Mona job Description'  
execute spInsertJob 'Mina Job',100,'Mina Job Description'  
execute spInsertJob 'Farshid Job',100,'Farshid Job Description'  
execute spInsertJob 'Farimehr Job',100,'Farimehr Job Description'  

-- SPLIT

execute spInsertPerson 'Mani','Moayedi',37,1  
execute spInsertPerson 'Mona','Moayedi',42,2  
execute spInsertPerson 'Mina','Haddadi',55,3  
execute spInsertPerson 'Farshid','Moayedi',63,4  
execute spInsertPerson 'Farimehr','Ayerian',21,5  

-- SPLIT

execute spInsertAddress 'Tehran','Karimkh','First',1,1,1  
execute spInsertAddress 'Tehran','Saee','Second',2,2,1  

execute spInsertAddress 'Tehran','Karimkh','First',1,1,2  
execute spInsertAddress 'Tehran','Saee','Second',2,2,2

execute spInsertAddress 'Tehran','Karimkh','First',1,1,3
execute spInsertAddress 'Tehran','Saee','Second',2,2,3

execute spInsertAddress 'Tehran','Karimkh','First',1,1,4
execute spInsertAddress 'Tehran','Saee','Second',2,2,4

execute spInsertAddress 'Tehran','Karimkh','First',1,1,5
execute spInsertAddress 'Tehran','Saee','Second',2,2,5  

-- SPLIT


    
 