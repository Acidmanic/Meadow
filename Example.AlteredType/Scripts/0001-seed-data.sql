
Drop table Person;

create table Persons(
                       Id bigint AUTO_INCREMENT PRIMARY KEY ,
                       Name nvarchar(100),
                       Surname nvarchar(100),
                       Age int(10),
                       JobId bigint(16),
                        FOREIGN KEY (JobId) REFERENCES Jobs(Id)
);

--SPLIT

INSERT INTO Jobs (Title,IncomeInRials,JobDescription,Color) VALUES ('SimpleEmployee',100000,'Simple',0);
INSERT INTO Jobs (Title,IncomeInRials,JobDescription,Color) VALUES ('Developer',100000,'Code Code Code',2);
INSERT INTO Jobs (Title,IncomeInRials,JobDescription,Color) VALUES ('Project Manager',100000,'Plan Plan Plan',3);


INSERT INTO Persons (Name,Surname,Age,JobId) VALUES ('Mani','Moayedi',37,2);
INSERT INTO Persons (Name,Surname,Age,JobId) VALUES ('Mona','Moayedi',38,3);
