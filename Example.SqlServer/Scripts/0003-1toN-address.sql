create table Addresses
(
    City        nvarchar(100),
    Street      nvarchar(100),
    AddressName nvarchar(100),
    Block       int,
    Plate       int,
    Id          bigint IDENTITY (1,1) NOT NULL PRIMARY KEY,
    PersonId    bigint FOREIGN KEY REFERENCES Persons (Id)
)

go


create procedure spInsertAddress(@City nvarchar(100),
                                 @Street nvarchar(100),
                                 @AddressName nvarchar(100),
                                 @Block int,
                                 @Plate int,
                                 @Id bigint,
                                 @PersonId bigint) as
insert into Addresses (City, Street, AddressName, Block, Plate, PersonId)
values (@City, @Street, @AddressName, @Block, @Plate, @PersonId)
declare @NewId bigint = (IDENT_CURRENT('Addresses'));
select *
from Addresses
where Id = @NewId;


go



-- Seed some
insert into Addresses (City, Street, AddressName, Block, Plate, PersonId)
values ('Tehran', 'FirstSt', 'Home', 1, 12, 1)

insert into Addresses (City, Street, AddressName, Block, Plate, PersonId)
values ('Tehran', 'SecondSt', 'Work', 1, 14, 1)


go


alter procedure spGetPersonByIdFullTree(@Id bigint)
as
select P.Id       'Persons_Id',
       Name,
       Surname,
       Age,
       JobId,
       J.Id       'Jobs_Id',
       Title,
       IncomeInRials,
       JobDescription,
       City,
       Street,
       AddressName,
       Block,
       Plate,
       A.Id       'Addresses_Id',
       A.PersonId 'PersonId'
from Persons P
         join Jobs J on P.JobId = J.Id
         left join Addresses A on P.Id = A.PersonId
where P.Id = @Id
go


alter procedure spGetAllPersonsFullTree
as
select P.Id       'Persons_Id',
       Name,
       Surname,
       Age,
       JobId,
       J.Id       'Jobs_Id',
       Title,
       IncomeInRials,
       JobDescription,
       City,
       Street,
       AddressName,
       Block,
       Plate,
       A.Id       'Addresses_Id',
       A.PersonId 'PersonId'
from Persons P
         join Jobs J on P.JobId = J.Id
         left join Addresses A on P.Id = A.PersonId
go

