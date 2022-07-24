create table Tags(
                     PropertyId bigint,
                     ProductClassId bigint
)
--SPLIT

create table Jobs
(
    Id             bigint(16) AUTO_INCREMENT PRIMARY KEY,
    Title          nvarchar(100),
    IncomeInRials  bigint(16),
    JobDescription nvarchar(128)
)

--SPLIT

create table Person(
                     Id bigint(16) AUTO_INCREMENT PRIMARY KEY,
                     Name nvarchar(100),
                     Surname nvarchar(100),
                     Age int(16),
                     JobId bigint(16),
                    FOREIGN KEY (JobId) REFERENCES Jobs(Id)
)

--SPLIT