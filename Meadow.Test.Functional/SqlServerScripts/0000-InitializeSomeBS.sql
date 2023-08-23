
-- Crud Meadow.Test.Functional.Models.Address

-- Crud Meadow.Test.Functional.Models.Job

-- Crud Meadow.Test.Functional.Models.Person

-- Filtering Meadow.Test.Functional.Models.Person



-- {{Crud Meadow.Test.Functional.Models.Address}}

-- {{Crud Meadow.Test.Functional.Models.Job}}

-- {{Crud Meadow.Test.Functional.Models.Person}}

-- SPLIT


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

-- Filtering Meadow.Test.Functional.Models.Person
-- ---------------------------------------------------------------------------------------------------------------------
-- CrudTableScriptGenerator
-- ---------------------------------------------------------------------------------------------------------------------
IF OBJECT_ID(N'FilterResults', N'U') IS NULL
CREATE TABLE FilterResults
(
    Id bigint NOT NULL PRIMARY KEY IDENTITY(1,1),FilterHash nvarchar(256),ResultId bigint,ExpirationTimeStamp bigint
);

-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------

-- ---------------------------------------------------------------------------------------------------------------------
-- FilteringProceduresGenerator
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spRemoveExpiredFilterResults(@ExpirationTimeStamp BIGINT) AS
DELETE FROM FilterResults WHERE FilterResults.ExpirationTimeStamp >= @ExpirationTimeStamp
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spPerformPersonsFilterIfNeeded(@FilterHash NVARCHAR(128),
                                                @ExpirationTimeStamp BIGINT,
                                                @WhereClause NVARCHAR(1024)) AS
BEGIN
    IF (SELECT Count(Id) from FilterResults where FilterResults.FilterHash=@FilterHash) = 0

        declare @query nvarchar(1600) = CONCAT(
                'INSERT INTO FilterResults (FilterHash,ResultId,ExpirationTimeStamp)',
                'SELECT ''',@FilterHash,''',Id, ',@ExpirationTimeStamp,' FROM Persons ' , @WhereClause);
    execute sp_executesql @query
END
SELECT FilterResults.* FROM FilterResults WHERE FilterResults.FilterHash=FilterHash;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPersonsChunk(@Offset BIGINT,
                                    @Size BIGINT,
                                    @FilterHash nvarchar(128)) AS
    ;WITH Results_CTE AS
              (
                  SELECT
                      PersonsFullTree.*,
                      ROW_NUMBER() OVER (ORDER BY PersonsFullTree.Persons_Id) AS RowNum
                  FROM PersonsFullTree INNER JOIN FilterResults on PersonsFullTree.Persons_Id = FilterResults.ResultId
                  WHERE FilterResults.FilterHash=@FilterHash
              )
     SELECT *
     FROM Results_CTE
     WHERE RowNum >= (@Offset+1)
       AND RowNum < (@Offset+1) + @Size
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- </Filtering>
-- ---------------------------------------------------------------------------------------------------------------------
    
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
