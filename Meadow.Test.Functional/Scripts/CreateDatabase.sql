


-- C
--     Id,
--     Name,
--     AId,
--     A
--         Id,
--         Name,
--         Bs -> B
--               Id,
--               Name,
--               AId

create table A (
    Id bigint primary key not null identity (1,1),
    Name nvarchar(100)
)


create table B (
    Id bigint primary key not null identity (1,1),
    Name nvarchar(100),
    AId bigint,
    foreign key(AId) references A(Id)
)


create table C (
    Id bigint primary key not null identity (1,1),
    Name nvarchar(100),
    AId bigint,
    foreign key(AId) references A(Id)
)


insert into A (Name) values ('A1')
insert into A (Name) values ('A2')


insert into B (Name,AId) values ('Mani',1)
insert into B (Name,AId) values ('Mona',1)
insert into B (Name,AId) values ('Mina',2)
insert into B (Name,AId) values ('Farshid',2)

insert into C (Name,AId) values ('C1',1)
insert into C (Name,AId) values ('C2',2)


create view CFull as 
    select 
        C.Id 'C_Id',
        C.Name 'C_Name',
        C.AId 'C_AId',
        B.Id 'B_Id',
        B.Name 'B_Name',
        B.AId 'B_AId',
        A.Id 'A_Id',
        A.Name 'A_Name'
        from C
        join A on C.AId = A.Id
        join B on A.Id = B.AId
        
        
select * from CFull