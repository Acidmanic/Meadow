
-- Crud Meadow.Test.Functional.Models.Address

-- Crud Meadow.Test.Functional.Models.Job

-- Crud Meadow.Test.Functional.Models.Person

-- Filtering Meadow.Test.Functional.Models.Person

-- SPLIT 

-- Crud Meadow.Test.Functional.Models.Address
-- ---------------------------------------------------------------------------------------------------------------------
-- TableScriptGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE Addresses
(
    Id BIGINT(16) NOT NULL PRIMARY KEY AUTO_INCREMENT,City varchar(256),Street varchar(256),AddressName varchar(256),Block INT(10),Plate INT(10),PersonId BIGINT(16)
);
-- ---------------------------------------------------------------------------------------------------------------------
-- InsertProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spInsertAddress(IN City varchar(256),IN Street varchar(256),IN AddressName varchar(256),IN Block INT(10),IN Plate INT(10),IN PersonId BIGINT(16))
BEGIN
    INSERT INTO Addresses (City,Street,AddressName,Block,Plate,PersonId) VALUES (City,Street,AddressName,Block,Plate,PersonId);
    SET @nid = (select LAST_INSERT_ID());
    SELECT * FROM Addresses WHERE Addresses.Id=@nid;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ReadProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAddressById(IN Id BIGINT(16))
BEGIN
    SELECT * FROM Addresses WHERE Addresses.Id = Id  ;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ReadProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllAddresses()
BEGIN
    SELECT * FROM Addresses   ;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- DeleteProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spDeleteAddressById(IN Id BIGINT(16))
BEGIN
    DELETE FROM Addresses WHERE Addresses.Id=Id;
    SELECT TRUE Success;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- DeleteProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spDeleteAllAddresses()
BEGIN
    DELETE FROM Addresses;
    SELECT TRUE Success;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- UpdateProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spUpdateAddress(IN City varchar(256),IN Street varchar(256),IN AddressName varchar(256),IN Block INT(10),IN Plate INT(10),IN Id BIGINT(16),IN PersonId BIGINT(16))
BEGIN
    UPDATE Addresses SET City=City,Street=Street,AddressName=AddressName,Block=Block,Plate=Plate,PersonId=PersonId WHERE Addresses.Id=Id;
    SELECT * FROM Addresses WHERE Addresses.Id=Id;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- SaveProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spSaveAddress(IN City varchar(256),IN Street varchar(256),IN AddressName varchar(256),IN Block INT(10),IN Plate INT(10),IN Id BIGINT(16),IN PersonId BIGINT(16))
BEGIN
    IF EXISTS(SELECT 1 FROM Addresses WHERE Addresses.Id = Id) then

        UPDATE Addresses SET City=City,Street=Street,AddressName=AddressName,Block=Block,Plate=Plate,PersonId=PersonId WHERE Addresses.Id = Id;

        SELECT * FROM Addresses WHERE Addresses.Id = Id ORDER BY Id ASC LIMIT 1;

    ELSE
        INSERT INTO Addresses (City,Street,AddressName,Block,Plate,PersonId) VALUES (City,Street,AddressName,Block,Plate,PersonId);
        SET @nid = (select LAST_INSERT_ID());
        SELECT * FROM Addresses WHERE Addresses.Id = @nid;
    END IF;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- </Crud>
-- ---------------------------------------------------------------------------------------------------------------------


-- Crud Meadow.Test.Functional.Models.Job
-- ---------------------------------------------------------------------------------------------------------------------
-- TableScriptGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE Jobs
(
    Id BIGINT(16) NOT NULL PRIMARY KEY AUTO_INCREMENT,Title varchar(256),IncomeInRials BIGINT(16),JobDescription varchar(256)
);
-- ---------------------------------------------------------------------------------------------------------------------
-- InsertProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spInsertJob(IN Title varchar(256),IN IncomeInRials BIGINT(16),IN JobDescription varchar(256))
BEGIN
    INSERT INTO Jobs (Title,IncomeInRials,JobDescription) VALUES (Title,IncomeInRials,JobDescription);
    SET @nid = (select LAST_INSERT_ID());
    SELECT * FROM Jobs WHERE Jobs.Id=@nid;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ReadProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadJobById(IN Id BIGINT(16))
BEGIN
    SELECT * FROM Jobs WHERE Jobs.Id = Id  ;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ReadProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllJobs()
BEGIN
    SELECT * FROM Jobs   ;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- DeleteProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spDeleteJobById(IN Id BIGINT(16))
BEGIN
    DELETE FROM Jobs WHERE Jobs.Id=Id;
    SELECT TRUE Success;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- DeleteProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spDeleteAllJobs()
BEGIN
    DELETE FROM Jobs;
    SELECT TRUE Success;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- UpdateProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spUpdateJob(IN Id BIGINT(16),IN Title varchar(256),IN IncomeInRials BIGINT(16),IN JobDescription varchar(256))
BEGIN
    UPDATE Jobs SET Title=Title,IncomeInRials=IncomeInRials,JobDescription=JobDescription WHERE Jobs.Id=Id;
    SELECT * FROM Jobs WHERE Jobs.Id=Id;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- SaveProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spSaveJob(IN Id BIGINT(16),IN Title varchar(256),IN IncomeInRials BIGINT(16),IN JobDescription varchar(256))
BEGIN
    IF EXISTS(SELECT 1 FROM Jobs WHERE Jobs.Id = Id) then

        UPDATE Jobs SET Title=Title,IncomeInRials=IncomeInRials,JobDescription=JobDescription WHERE Jobs.Id = Id;

        SELECT * FROM Jobs WHERE Jobs.Id = Id ORDER BY Id ASC LIMIT 1;

    ELSE
        INSERT INTO Jobs (Title,IncomeInRials,JobDescription) VALUES (Title,IncomeInRials,JobDescription);
        SET @nid = (select LAST_INSERT_ID());
        SELECT * FROM Jobs WHERE Jobs.Id = @nid;
    END IF;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- </Crud>
-- ---------------------------------------------------------------------------------------------------------------------


-- Crud Meadow.Test.Functional.Models.Person
-- ---------------------------------------------------------------------------------------------------------------------
-- TableScriptGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE Persons
(
    Id BIGINT(16) NOT NULL PRIMARY KEY AUTO_INCREMENT,Name varchar(256),Surname varchar(256),Age INT(10),JobId BIGINT(16)
);
-- ---------------------------------------------------------------------------------------------------------------------
-- InsertProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spInsertPerson(IN Name varchar(256),IN Surname varchar(256),IN Age INT(10),IN JobId BIGINT(16))
BEGIN
    INSERT INTO Persons (Name,Surname,Age,JobId) VALUES (Name,Surname,Age,JobId);
    SET @nid = (select LAST_INSERT_ID());
    SELECT * FROM Persons WHERE Persons.Id=@nid;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ReadProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPersonById(IN Id BIGINT(16))
BEGIN
    SELECT * FROM Persons WHERE Persons.Id = Id  ;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ReadProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadAllPersons()
BEGIN
    SELECT * FROM Persons   ;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- DeleteProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spDeletePersonById(IN Id BIGINT(16))
BEGIN
    DELETE FROM Persons WHERE Persons.Id=Id;
    SELECT TRUE Success;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- DeleteProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spDeleteAllPersons()
BEGIN
    DELETE FROM Persons;
    SELECT TRUE Success;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- UpdateProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spUpdatePerson(IN Id BIGINT(16),IN Name varchar(256),IN Surname varchar(256),IN Age INT(10),IN JobId BIGINT(16))
BEGIN
    UPDATE Persons SET Name=Name,Surname=Surname,Age=Age,JobId=JobId WHERE Persons.Id=Id;
    SELECT * FROM Persons WHERE Persons.Id=Id;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- SaveProcedureGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spSavePerson(IN Id BIGINT(16),IN Name varchar(256),IN Surname varchar(256),IN Age INT(10),IN JobId BIGINT(16))
BEGIN
    IF EXISTS(SELECT 1 FROM Persons WHERE Persons.Id = Id) then

        UPDATE Persons SET Name=Name,Surname=Surname,Age=Age,JobId=JobId WHERE Persons.Id = Id;

        SELECT * FROM Persons WHERE Persons.Id = Id ORDER BY Id ASC LIMIT 1;

    ELSE
        INSERT INTO Persons (Name,Surname,Age,JobId) VALUES (Name,Surname,Age,JobId);
        SET @nid = (select LAST_INSERT_ID());
        SELECT * FROM Persons WHERE Persons.Id = @nid;
    END IF;
END;
-- ---------------------------------------------------------------------------------------------------------------------
create view PersonsFullTree as
select
    Persons.Id 'Persons_Id',
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
-- ---------------------------------------------------------------------------------------------------------------------
-- </Crud>
-- ---------------------------------------------------------------------------------------------------------------------


-- Filtering Meadow.Test.Functional.Models.Person
-- ---------------------------------------------------------------------------------------------------------------------
-- TableScriptGenerator
-- ---------------------------------------------------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS FilterResults
(
    Id BIGINT(16) NOT NULL PRIMARY KEY AUTO_INCREMENT,FilterHash varchar(256),ResultId BIGINT(16),ExpirationTimeStamp BIGINT(16)
);
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
    SELECT FilterResults.* FROM FilterResults WHERE FilterResults.FilterHash=FilterHash;
END;
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spReadPersonsChunk(IN Offset bigint(16),
                                    IN Size bigint(16),
                                    IN FilterHash nvarchar(128))
BEGIN
    select PersonsFullTree.* from PersonsFullTree 
        inner join (select FilterResults.* from FilterResults limit offset,size) FR 
        on PersonsFullTree.Persons_Id = FR.ResultId
    where FR.FilterHash=FilterHash;

END;
-- ---------------------------------------------------------------------------------------------------------------------
-- </Filtering>
-- ---------------------------------------------------------------------------------------------------------------------

-- SPLIT

call spInsertJob('Mani Job',100,'Mani job Description');
call spInsertJob('Mona Job',100,'Mona job Description');
call spInsertJob('Mina Job',100,'Mina Job Description');
call spInsertJob('Farshid Job',100,'Farshid Job Description');
call spInsertJob('Farimehr Job',100,'Farimehr Job Description');

-- SPLIT

call spInsertPerson('Mani','Moayedi',37,1);
call spInsertPerson('Mona','Moayedi',42,2);
call spInsertPerson('Mina','Haddadi',55,3);
call spInsertPerson('Farshid','Moayedi',63,4);
call spInsertPerson('Farimehr','Ayerian',21,5);

-- SPLIT

call spInsertAddress('Tehran','Karimkh','First',1,1,1);
call spInsertAddress('Tehran','Saee','Second',2,2,1);

call spInsertAddress('Tehran','Karimkh','First',1,1,2);
call spInsertAddress('Tehran','Saee','Second',2,2,2);

call spInsertAddress('Tehran','Karimkh','First',1,1,3);
call spInsertAddress('Tehran','Saee','Second',2,2,3);

call spInsertAddress('Tehran','Karimkh','First',1,1,4);
call spInsertAddress('Tehran','Saee','Second',2,2,4);

call spInsertAddress('Tehran','Karimkh','First',1,1,5);
call spInsertAddress('Tehran','Saee','Second',2,2,5);

-- SPLIT