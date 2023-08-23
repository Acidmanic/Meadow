
use master
drop database LitbidMeadowDb
create database LitbidMeadowDb
USE LitbidMeadowDb
Go

-- Crud Example.SqlServer.Models.Person
-- ---------------------------------------------------------------------------------------------------------------------
-- CrudTableScriptGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE Persons
(
    Id bigint NOT NULL PRIMARY KEY IDENTITY(1,1),Name nvarchar(64),Surname nvarchar(128),Age int,JobId bigint
);
GO

-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------

-- ---------------------------------------------------------------------------------------------------------------------
-- InsertProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE spInsertPerson(@Name nvarchar(64),@Surname nvarchar(128),@Age int,@JobId bigint) AS
    
    INSERT INTO Persons (Name,Surname,Age,JobId) 
                   VALUES (@Name,@Surname,@Age,@JobId)
    DECLARE @newId bigint=(IDENT_CURRENT('Persons'));
    SELECT * FROM Persons WHERE Id=@newId;
GO

-- ---------------------------------------------------------------------------------------------------------------------
-- ReadProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPersonById(@Id bigint) AS
    SELECT * FROM Persons WHERE Persons.Id = @Id;
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- ReadProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersons AS
    SELECT * FROM Persons;
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- DeleteProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE spDeletePersonById( @Id bigint) AS
    DECLARE @existing int = (SELECT COUNT(*) FROM Persons);
    DELETE FROM Persons WHERE Id=@Id
    DECLARE @delta int = @existing - (SELECT COUNT(*) FROM Persons);
    IF @delta > 0 or @existing = 0
        SELECT cast(1 as bit) Success
    ELSE
        select cast(0 as bit) Success
GO

-- ---------------------------------------------------------------------------------------------------------------------
-- DeleteProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE spDeleteAllPersons AS
    DECLARE @existing int = (SELECT COUNT(*) FROM Persons);
    DELETE FROM Persons 
    DECLARE @delta int = @existing - (SELECT COUNT(*) FROM Persons);
    IF @delta > 0 or @existing = 0
        SELECT cast(1 as bit) Success
    ELSE
        select cast(0 as bit) Success
GO

-- ---------------------------------------------------------------------------------------------------------------------
-- UpdateProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spUpdatePerson(@Id bigint,@Name nvarchar(64),@Surname nvarchar(128),@Age int,@JobId bigint) AS
    UPDATE Persons
    SET Name = @Name,Surname = @Surname,Age = @Age,JobId = @JobId
    WHERE Id=@Id;
    
    SELECT * FROM Persons WHERE Id=@Id;
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- SaveProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spSavePerson(@Id bigint,@Name nvarchar(64),@Surname nvarchar(128),@Age int,@JobId bigint) AS
    IF EXISTS(SELECT 1 FROM Persons WHERE Persons.Id = @Id)
        BEGIN
            UPDATE Persons SET Name= @Name,Surname= @Surname,Age= @Age,JobId= @JobId WHERE Persons.Id = @Id;
        
            SELECT TOP 1 * FROM Persons WHERE Persons.Id = @Id ORDER BY Id ASC;
        END
    ELSE
        BEGIN
            INSERT INTO Persons (Name,Surname,Age,JobId) VALUES (@Name,@Surname,@Age,@JobId);

            DECLARE @newId bigint=(IDENT_CURRENT('Persons'));

            SELECT * FROM Persons WHERE Persons.Id = @newId;
        END
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- </Crud>
-- ---------------------------------------------------------------------------------------------------------------------


GO
-- FilterResultsTable
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
-- </FilterResultsTable>
-- ---------------------------------------------------------------------------------------------------------------------

GO
-- FilteringProcedures Example.SqlServer.Models.Person
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
            'INSERT INTO FilterResults (FilterHash,ResultId,ExpirationTimeStamp)',CHAR(13),
            'SELECT ''',@FilterHash,''',Id, ',@ExpirationTimeStamp,' FROM Persons' , @WhereClause,CHAR(13),'GO');
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
                      Persons.*,
                      ROW_NUMBER() OVER (ORDER BY Persons.Id) AS RowNum
                  FROM Persons INNER JOIN FilterResults on Persons.Id = FilterResults.ResultId
                  WHERE FilterResults.FilterHash=@FilterHash
              )
     SELECT Id,Name,Surname,Age,JobId
     FROM Results_CTE
     WHERE RowNum >= @Offset
       AND RowNum < @Offset + @Size
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- </FilteringProcedures>
-- ---------------------------------------------------------------------------------------------------------------------



execute spInsertPerson 'Mani','Moayedi',37,1
execute spInsertPerson 'Mona','Moayedi',42,2
execute spInsertPerson 'Mina','Haddadi',56,3
execute spInsertPerson 'Farshid','Moayedi',63,4
execute spInsertPerson 'Farimehr','Ayerian',21,5

execute spPerformPersonsFilterIfNeeded 'mash',12345,''

execute spReadPersonsChunk 2,3,'mash'
