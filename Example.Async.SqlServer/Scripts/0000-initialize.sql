create table Tags(
                     PropertyId bigint,
                     ProductClassId bigint
)

create table Jobs
(
    Id             bigint IDENTITY (1,1) NOT NULL PRIMARY KEY,
    Title          nvarchar(100),
    IncomeInRials  bigint,
    JobDescription nvarchar(128)
)

create table Person(
                     Id bigint IDENTITY (1,1) NOT NULL PRIMARY KEY ,
                     Name nvarchar(100),
                     Surname nvarchar(100),
                     Age int,
                     JobId bigint FOREIGN KEY REFERENCES Jobs(Id)
)
