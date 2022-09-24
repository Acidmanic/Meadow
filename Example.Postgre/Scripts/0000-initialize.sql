create table "Tags"(
                     "PropertyId" INTEGER,
                     "ProductClassId" INTEGER
);

create table "Jobs" ("Title" TEXT, "IncomeInRials" INTEGER,"JobDescription" TEXT,"Id" SERIAL, PRIMARY KEY ("Id"));

create table "Person"(
                        "Id" SERIAL ,
                        "Name" TEXT,
                        "Surname" TEXT,
                        "Age" INTEGER,
                        "JobId" INTEGER,
                        PRIMARY KEY("Id"),
                        FOREIGN KEY("JobId") REFERENCES "Jobs"("Id")
);