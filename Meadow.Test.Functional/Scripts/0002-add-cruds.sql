    
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
    select * from Persons P join Jobs J on J.Id = P.JobId where P.Id = @Id
go

create procedure spGetAllJobs
as 
    select * from Jobs
go

create procedure spGetJobById(@Id bigint)
as
    select * from Jobs where Id = @Id
go
