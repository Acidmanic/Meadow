    
create procedure spGetAllPersons
    as
        select * from Persons;
go


create procedure spGetPersonById(@Id bigint)
as
    select * from Persons where Id = @Id
go


create procedure spGetAllEager
as
    select * from Persons join Jobs J on J.Id = Persons.JobId
go

create procedure spGetPersonByIdEager(@Id bigint)
as
    select P.Id  'Persons.Id',
           Name, Surname,Age,JobId,J.Id 'Jobs.Id',
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