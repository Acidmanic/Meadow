Drop Table If EXISTS  Addresses;
-- ---------------------------------------------------------------------------------------------------------------------
Drop Table If EXISTS  Persons;
-- ---------------------------------------------------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS spPerformPersonsFilterIfNeeded;
-- ---------------------------------------------------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS spReadPersonsChunk;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE Persons(
                        Id      bigint auto_increment
                            primary key,
                        Name    varchar(100) charset utf8mb3 null,
                        Surname varchar(100) charset utf8mb3 null,
                        Age     int                          null,
                        JobId   bigint                       null
);
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- FilterResultsTable
-- ---------------------------------------------------------------------------------------------------------------------
-- TableScriptGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS FilterResults
(
    Id BIGINT(16) NOT NULL PRIMARY KEY AUTO_INCREMENT,FilterHash varchar(256),ResultId BIGINT(16),ExpirationTimeStamp BIGINT(16)
);
-- ---------------------------------------------------------------------------------------------------------------------
-- </FilterResultsTable>
-- ---------------------------------------------------------------------------------------------------------------------

-- ---------------------------------------------------------------------------------------------------------------------
-- FilteringProcedures Example.MySql.Models.Person
-- ---------------------------------------------------------------------------------------------------------------------
-- FilteringProceduresGenerator
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS spRemoveExpiredFilterResults;
CREATE PROCEDURE spRemoveExpiredFilterResults(IN ExpirationTimeStamp bigint(16))
BEGIN
    DELETE FROM FilterResults WHERE FilterResults.ExpirationTimeStamp >= ExpirationTimeStamp;
END;
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spPerformPersonsFilterIfNeeded(IN FilterHash nvarchar(128),
                                                  IN ExpirationTimeStamp bigint(16),
                                                  IN WhereClause nvarchar(1024))
BEGIN
    if not exists(select 1 from FilterResults where FilterResults.FilterHash=FilterHash) then
        set @query = CONCAT(
            'insert into FilterResults (FilterHash,ResultId,ExpirationTimeStamp)',
            'select \'',FilterHash,'\',Persons.Id,',ExpirationTimeStamp,' from Persons ' , WhereClause,';');
        PREPARE stmt FROM @query;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
            
    end if;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPersonsChunk(IN Offset bigint(16),
                                      IN Size bigint(16),
                                      IN FilterHash nvarchar(128))
BEGIN
    select Persons.* from Persons inner join FilterResults on Persons.Id = FilterResults.ResultId
    where FilterResults.FilterHash=FilterHash limit offset,size;  
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- </FilteringProcedures>
-- ---------------------------------------------------------------------------------------------------------------------

-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
delete from Persons where true;
delete from FilterResults where true;
insert into Persons (Name, Surname, Age, JobId) values ('Mani','Moayedi',37,1);
insert into Persons (Name, Surname, Age, JobId) values ('Mona','Moayedi',41,2);
insert into Persons (Name, Surname, Age, JobId) values ('Mina','Haddadi',56,3);
insert into Persons (Name, Surname, Age, JobId) values ('Farshid','Moayedi',63,3);
-- ---------------------------------------------------------------------------------------------------------------------
call spRemoveExpiredFilterResults(1000);


set @where = 'where (Name = \'Mina\' OR Name = \'Farshid\') AND (Age >= 58)';
set @hash = md5(@`where`);

call spPerformPersonsFilterIfNeeded(@hash,1000,@where);

call spReadPersonsChunk(0,10,@hash);
