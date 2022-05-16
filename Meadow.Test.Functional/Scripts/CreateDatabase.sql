
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



execute spInsertTag 9,99