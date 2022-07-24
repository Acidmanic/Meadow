
CREATE database MeadowDatabase

create table Tags(
    PropertyId bigint,
    ProductClassId bigint
)

create procedure spReadAllTags
as
    begin 
        select * from Tags
    end

        
insert into Tags (PropertyId,ProductClassId)  values (30,40)  


create procedure spInsertTag(
    @PropertyId bigint,
    @ProductClassId bigint
)
as 
    begin
        insert into Tags (PropertyId,ProductClassId)  values (@PropertyId,@ProductClassId)
    end



DROP DATABASE Mexghun

create DATABASE Mexghun


IF (DB_ID('Mexghun') IS NOT NULL)
    select cast(1 as bit) Ex 
ELSE 
    select cast(0 as bit) Ex


    CREATE TABLE MeadowDatabaseHistorys (
                                            Id bigint, ScriptOrder bigint, ScriptName nvarchar(256), Script nvarchar(256)
    )


SELECT TOP 1 * FROM MeadowDatabaseHistorys ORDER BY Id DESC WHERE Id=@Id

