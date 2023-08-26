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
-- FilteringProcedures Example.MySql.Models.Person
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


drop  procedure spPerformPersonsFilterIfNeeded;
create  procedure spPerformPersonsFilterIfNeeded(IN FilterHash varchar(128), IN SearchId varchar(32),
                                                                IN ExpirationTimeStamp bigint,
                                                                IN FilterExpression varchar(1024))
BEGIN
    if not exists(select 1 from FilterResults where FilterResults.SearchId=SearchId) then
        IF FilterExpression IS NULL OR FilterExpression = '' THEN
            set FilterExpression = 'TRUE';
        END IF;
        set @query = CONCAT(
                'insert into FilterResults (FilterHash,SearchId,ResultId,ExpirationTimeStamp)',
                'select \'',FilterHash,'\',\'',SearchId,'\',Persons.Id,',ExpirationTimeStamp,
                ' from Persons WHERE ' , FilterExpression,';');
        PREPARE stmt FROM @query;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    end if;
    SELECT FilterResults.* FROM FilterResults WHERE FilterResults.SearchId=SearchId;
END;


set @where = '(Name = \'Mina\' OR Name = \'Farshid\') AND (Age >= 58)';
set @hash = md5(@`where`);

call spPerformPersonsFilterIfNeeded(@hash,'sssss',1000,@where);

call spReadPersonsChunk(0,10,@hash);
