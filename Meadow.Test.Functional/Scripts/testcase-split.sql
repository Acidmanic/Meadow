CREATE TABLE UserInformationDals
(
    Id           nvarchar(64) PRIMARY KEY,
    Name         nvarchar(256),
    Surname      nvarchar(256),
    Email        nvarchar(128),
    NationalCode nvarchar(16),
    MobileNumber nvarchar(16),
    PhoneNumber  nvarchar(16),
    PostalCode   nvarchar(16),
    RealPerson   bit,
    Address      nvarchar(256),
    CompanyName  nvarchar(32),
    EconomicCode nvarchar(32),
    YearOfBirth  int
)

-- --SPLIT
-- 
-- INSERT INTO UserInformationDals (Id,Name, Surname, Email, NationalCode, MobileNumber, PhoneNumber, PostalCode,
--                                  RealPerson, Address, CompanyName, EconomicCode, YearOfBirth)
--                                  VALUES ('04844ac5-9fba-4371-a036-1ab9cbea8ae2','Mani','Moayedi','acidmanic.moayedi@gmail.com',
--                                          '0003003003','09764765648','098834746854','0000000020',1,'meeh','acidmanic',
--                                          '0000400','1985')
-- 
--         
--SPLIT

CREATE PROCEDURE spReadAllUserInformationDals
    AS
SELECT *
FROM UserInformationDals
    GO


CREATE PROCEDURE spInsertUserInformationDal(@Id nvarchar(64) , @Name nvarchar(256), @Surname nvarchar(256),
                                            @Email nvarchar(128),
                                            @NationalCode nvarchar(16), @MobileNumber nvarchar(16),
                                            @PhoneNumber nvarchar(16), @PostalCode nvarchar(16), @RealPerson bit,
                                            @Address nvarchar(256), @CompanyName nvarchar(32),
                                            @EconomicCode nvarchar(32), @YearOfBirth int)
    AS
INSERT INTO UserInformationDals
(Id,Name, Surname, Email, NationalCode, MobileNumber, PhoneNumber, PostalCode,
 RealPerson, Address, CompanyName, EconomicCode, YearOfBirth)
OUTPUT inserted.* 
VALUES (@Id,@Name, @Surname, @Email, @NationalCode, @MobileNumber, @PhoneNumber, @PostalCode,
        @RealPerson, @Address, @CompanyName, @EconomicCode, @YearOfBirth);

GO



CREATE  PROCEDURE spReadUserInformationDalById(@Id nvarchar(64))
    AS
SELECT  * FROM UserInformationDals  WHERE Id=@Id
    GO


CREATE  PROCEDURE spDeleteUserInformationDalById(@Id nvarchar(64))
    AS
DECLARE @existing int = (SELECT COUNT(*) FROM UserInformationDals);
DELETE FROM UserInformationDals WHERE Id=@Id
DECLARE @delta int = @existing - (SELECT COUNT(*) FROM UserInformationDals);
    IF @delta > 0 or @existing = 0
SELECT cast(1 as bit) Success
    ELSE
select cast(0 as bit) Success
    GO