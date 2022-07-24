create table Tags(
                     PropertyId INTEGER,
                     ProductClassId INTEGER
);

create table Jobs (Title TEXT, IncomeInRials INTEGER,JobDescription TEXT,Id INTEGER PRIMARY KEY);

create table Person(
                        Id INTEGER PRIMARY KEY ,
                        Name TEXT,
                        Surname TEXT,
                        Age INTEGER,
                        JobId INTEGER,
                        FOREIGN KEY(JobId) REFERENCES Jobs(Id)
);