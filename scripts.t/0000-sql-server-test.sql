
use master
drop database LitbidMeadowDb
create database LitbidMeadowDb
USE LitbidMeadowDb
Go

-- Table Example.SqlServer.Models.Person
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
-- </Table>
-- ---------------------------------------------------------------------------------------------------------------------


-- Insert Example.SqlServer.Models.Person
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
-- </Insert>
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
ALTER PROCEDURE spPerformPersonsFilterIfNeeded(@FilterHash NVARCHAR(128),
                                                  @ExpirationTimeStamp BIGINT,
                                                  @WhereClause NVARCHAR(1024)) AS
BEGIN
    IF (SELECT Count(Id) from FilterResults where FilterResults.FilterHash=@FilterHash) = 0
        SET @WhereClause = coalesce(nullif(@WhereClause, ''), '1=1')
        declare @query nvarchar(1600) = CONCAT(
            'INSERT INTO FilterResults (FilterHash,ResultId,ExpirationTimeStamp)',
            'SELECT ''',@FilterHash,''',Id, ',@ExpirationTimeStamp,' FROM Persons Where ' , @WhereClause);
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

declare @query nvarchar(256) = '(Name = ''Mani'' OR Name = ''Mona'') AND ( Age > 40)';
declare @hash nvarchar(128) = CONVERT(VARCHAR(32), HashBytes('MD5', @query), 2);

execute spPerformPersonsFilterIfNeeded @hash,'769d8af4443d11ee9592fb628444f815',12345,@query

execute spReadPersonsChunk 0,20,@hash

