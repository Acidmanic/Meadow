
-- Crud Meadow.Test.Functional.Models.Address

-- Crud Meadow.Test.Functional.Models.Job

-- Crud Meadow.Test.Functional.Models.Person

-- Filtering Meadow.Test.Functional.Models.Person

-- SPLIT 

-- {{Crud Meadow.Test.Functional.Models.Address}}
-- {{Crud Meadow.Test.Functional.Models.Job}}
-- {{Crud Meadow.Test.Functional.Models.Person}}
-- ---------------------------------------------------------------------------------------------------------------------
-- {{Filtering Meadow.Test.Functional.Models.Person}}
-- ---------------------------------------------------------------------------------------------------------------------

-- SPLIT

call spInsertJob('Mani Job',100,'Mani job Description');
call spInsertJob('Mona Job',100,'Mona job Description');
call spInsertJob('Mina Job',100,'Mina Job Description');
call spInsertJob('Farshid Job',100,'Farshid Job Description');
call spInsertJob('Farimehr Job',100,'Farimehr Job Description');

-- SPLIT

call spInsertPerson('Mani','Moayedi',37,1);
call spInsertPerson('Mona','Moayedi',42,2);
call spInsertPerson('Mina','Haddadi',55,3);
call spInsertPerson('Farshid','Moayedi',63,4);
call spInsertPerson('Farimehr','Ayerian',21,5);

-- SPLIT

call spInsertAddress('Tehran','Karimkh','First',1,1,1);
call spInsertAddress('Tehran','Saee','Second',2,2,1);

call spInsertAddress('Tehran','Karimkh','First',1,1,2);
call spInsertAddress('Tehran','Saee','Second',2,2,2);

call spInsertAddress('Tehran','Karimkh','First',1,1,3);
call spInsertAddress('Tehran','Saee','Second',2,2,3);

call spInsertAddress('Tehran','Karimkh','First',1,1,4);
call spInsertAddress('Tehran','Saee','Second',2,2,4);

call spInsertAddress('Tehran','Karimkh','First',1,1,5);
call spInsertAddress('Tehran','Saee','Second',2,2,5);

-- SPLIT