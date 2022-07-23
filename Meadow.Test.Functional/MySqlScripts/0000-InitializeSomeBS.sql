
create table Persons(Id bigint AUTO_INCREMENT primary key , Name nvarchar(100), Surname nvarchar(100));

insert into Persons (Name, Surname) values ('Mani','Moayedi');
insert into Persons (Name, Surname) values ('Farimehr','Ayerian');

create procedure spInsertPerson(IN Name nvarchar(100),IN Surname nvarchar(100))
begin
    insert into Persons (Name,Surname) values (Name,Surname);
    select * from Persons where Persons.Id=last_insert_id();
end

--SPLIT
    
create procedure spReadAllPersons()
    begin 
        select * from Persons;
    end