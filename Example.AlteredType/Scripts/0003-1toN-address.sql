create table Addresses
(
    City        nvarchar(100),
    Street      nvarchar(100),
    AddressName nvarchar(100),
    Block       int(10),
    Plate       int(10),
    Id          bigint(16) AUTO_INCREMENT PRIMARY KEY,
    PersonId    bigint(16) ,
    FOREIGN KEY (PersonId) REFERENCES Persons(Id)
);

--SPLIT

create procedure spInsertAddress(IN City nvarchar(100),
                                 IN Street nvarchar(100),
                                 IN AddressName nvarchar(100),
                                 IN Block int,
                                 IN Plate int,
                                 IN Id bigint,
                                 IN PersonId bigint)
begin
    
    insert into Addresses (City, Street, AddressName, Block, Plate, PersonId)
    values (City, Street, AddressName, Block, Plate, PersonId);
    select * from Addresses where Persons.Id=last_insert_id();

end;

--SPLIT

-- Seed some
insert into Addresses (City, Street, AddressName, Block, Plate, PersonId)
values ('Tehran', 'FirstSt', 'Home', 1, 12, 1);

insert into Addresses (City, Street, AddressName, Block, Plate, PersonId)
values ('Tehran', 'SecondSt', 'Work', 1, 14, 1);

--SPLIT

drop procedure spGetPersonByIdFullTree;
    
--SPLIT
    
    
create procedure spGetPersonByIdFullTree(Id bigint(16))
begin
    
select Persons.Id    'Persons_Id',
       Name,
       Surname,
       Age,
       JobId,
       Jobs.Id       'Jobs_Id',
       Jobs.Color,
       Title,
       IncomeInRials,
       JobDescription,
       City,
       Street,
       AddressName,
       Block,
       Plate,
       Addresses.Id       'Addresses_Id',
       Addresses.PersonId 'Addresses_PersonId'
       from Persons
         inner join Jobs on Persons.JobId = Jobs.Id
         left join Addresses on Persons.Id = Addresses.PersonId
           where Persons.Id = Id;
end;

--SPLIT

drop procedure spGetAllPersonsFullTree;
    
--SPLIT

create procedure spGetAllPersonsFullTree()
begin 
select P.Id       'Persons_Id',
       Name,
       Surname,
       Age,
       JobId,
       J.Id       'Jobs_Id',
       J.Color,
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
         inner join Jobs J on P.JobId = J.Id
         left join Addresses A on P.Id = A.PersonId;
end;

