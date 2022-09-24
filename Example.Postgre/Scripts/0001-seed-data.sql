
Drop table "Person";

create table "Persons"(
                        "Id" SERIAL,
                        "Name" TEXT,
                        "Surname" TEXT,
                        "Age" INTEGER,
                        "JobId" INTEGER,
                        PRIMARY KEY ("Id"),
                        FOREIGN KEY("JobId") REFERENCES "Jobs"("Id")
);

INSERT INTO "Jobs" ("Title","IncomeInRials","JobDescription") VALUES ('SimpleEmployee',100000,'Simple');
INSERT INTO "Jobs" ("Title","IncomeInRials","JobDescription") VALUES ('Developer',100000,'Code Code Code');
INSERT INTO "Jobs" ("Title","IncomeInRials","JobDescription") VALUES ('Project Manager',100000,'Plan Plan Plan');

    
INSERT INTO "Persons" ("Name","Surname","Age","JobId") VALUES ('Mani','Moayedi',37,3);
INSERT INTO "Persons" ("Name","Surname","Age","JobId") VALUES ('Mona','Moayedi',38,3);
