


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
