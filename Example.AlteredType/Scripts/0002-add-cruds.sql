

create procedure spGetPersonById(IN Id bigint(16))
begin
    select * from Persons where Id = Id;
end;

--SPLIT 
    
CREATE PROCEDURE spGetAllPersonsFullTree()
BEGIN
    SELECT Persons.Id 'Persons_Id', Name, Surname, Age, JobId,
        Jobs.Id 'Jobs_Id',Title,JobDescription,IncomeInRials
    FROM Persons INNER JOIN Jobs  ON Jobs.Id = Persons.JobId;
END;

--SPLIT

create procedure spGetPersonByIdFullTree(IN Id bigint(16))
BEGIN
    select P.Id  'Persons_Id',
           Name, Surname,Age,JobId,J.Id 'Jobs_Id',
           Title,IncomeInRials,JobDescription
        from Persons P inner join Jobs J on J.Id = P.JobId where P.Id = Id;
END;

--SPLIT

create procedure spGetAllJobs()
BEGIN
    select * from Jobs;
END;
    
--SPLIT

create procedure spGetJobById(Id bigint(16))
begin
    select * from Jobs where Id = Id;
end;

--SPLIT

create procedure spInsertPerson(
    IN Name nvarchar(100),
    IN Surname nvarchar(100),
    IN Age int(10),
    IN JobId bigint(16)
    )
begin
    insert into Persons (Name, Surname, Age, JobId) values (Name,Surname,Age,JobId);
    select * from Persons where Persons.Id=last_insert_id();
end;

--SPLIT    

create procedure spInsertJob(
        IN Title nvarchar(100),
        IN IncomeInRials bigint(16),
        IN JobDescription nvarchar(128),
        IN Color bigint
)
begin
    insert into Jobs (Title, IncomeInRials, JobDescription,Color) values (Title,IncomeInRials,JobDescription,Color);
    select * from Jobs where Jobs.Id=last_insert_id();
end;

--SPLIT    

create procedure spDeletePersonById(IN Id bigint(16)) 
BEGIN
    delete from Persons where Persons.Id=Id;
    select TRUE success;
END;
