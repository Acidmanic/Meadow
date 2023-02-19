    
create procedure spGetAllPersons
    as
        select * from Persons;
go


create procedure spGetPersonById(@Id bigint)
as
    select * from Persons where Id = @Id
go


create procedure spGetAllPersonsFullTree
as
    select
        P.Id 'Persons_Id', Name, Surname, Age, JobId,
           J.Id 'Jobs_Id',Title,JobDescription,IncomeInRials
    from Persons P join Jobs J on J.Id = P.JobId
go

create procedure spGetPersonByIdFullTree(@Id bigint)
as
    select P.Id  'Persons_Id',
           Name, Surname,Age,JobId,J.Id 'Jobs_Id',
           Title,IncomeInRials,JobDescription
        from Persons P join Jobs J on J.Id = P.JobId where P.Id = @Id
go

create procedure spGetAllJobs
as 
    select * from Jobs
go

create procedure spGetJobById(@Id bigint)
as
    select * from Jobs where Id = @Id
go

create procedure spInsertPerson(
    @Name nvarchar(100),
    @Surname nvarchar(100),
    @Age int,
    @JobId bigint
) as
    insert into Persons (Name, Surname, Age, JobId) values (@Name,@Surname,@Age,@JobId)
    declare @NewId bigint = (IDENT_CURRENT('Persons'));
    select * from Persons where Id=@NewId;
go

create procedure spInsertJob(
        @Title nvarchar(100),
        @IncomeInRials bigint,
        @JobDescription nvarchar(128)
)as 
    insert into Jobs (Title, IncomeInRials, JobDescription) values (@Title,@IncomeInRials,@JobDescription)  
    
go 
                                                            
                                                            

create procedure spDeletePersonById(@Id bigint) 
as
    delete from Persons where Persons.Id=@Id
    select cast(1 as bit) success
go


create procedure spDeleteJob(@Id bigint)
as
    if EXISTS(select Id from Persons where JobId=@Id)
    begin 
        delete from Jobs where Jobs.Id = @Id
        select cast(1 as bit) success
    end
else
    select cast(0 as bit) success
go


    
    

    select * from Jobs

    
    insert into Jobs (Title, IncomeInRials, JobDescription)  values ('Mani',1234,'Kar kon baba');
    SELECT last_insert_rowid();    

insert into Tags (PropertyId, ProductClassId) values (13,13);
SELECT last_insert_rowid();    
-- Hello 
select * from Tags where ROWID=LAST_INSERT_ROWID()
------------------------------------------------------------------------------------------------------------------------


create table Dudu(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT 
);

INSERT into Dudu (Name) values ('ChucChu')


update Dudu set Name='Sascha' where Dudu.Id=2;
    
select * from Dudu WHERE Id > 0 ORDER BY ROWID DESC LIMIT 2


INSERT into Dudu (Name) values ('DELETABLE')
PRAGMA temp_store = 2; /* 2 means use in-memory */
CREATE TEMP TABLE _Existing(Count INTEGER);
INSERT INTO _Existing (Count) SELECT COUNT(*) FROM Dudu;

SELECT * FROM _Existing;

--DELETE FROM Dudu WHERE Name = 'DELETABLE';

INSERT INTO _Existing (Count) SELECT COUNT(*) FROM Dudu;

SELECT CASE WHEN Count(DISTINCT Count)=2 THEN CAST(1 as bit) ELSE CAST(0 as bit) 
            END AS Success
FROM _Existing;

DROP TABLE _Existing;

DECLARE @existing int = (SELECT COUNT(*) FROM {_keyTableName});
DELETE FROM {_keyTableName};
DECLARE @delta int = @existing - (SELECT COUNT(*) FROM {_keyTableName})
    IF @delta > 0 OR @existing = 0
SELECT CAST(1 as bit) Success
    ELSE
SELECT CAST(0 as bit) Success