using System;
using System.Data.SqlClient;
using System.IO;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.BuildupScripts;
using Meadow.Requests;
using Meadow.SqlServer;
using Meadow.Test.Functional.TDDAbstractions;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd031InvalidGoSplit : MeadowFunctionalTest
    {

        public override void Main()
        {

            var script = @"----------------------------------------------------------------
----------------------------------------------------------------
----------------------------------------------------------------
create table PostCategories(
    Id bigint primary key identity(1,1),
    Name nvarchar(32),
    DefinitionUniqueId varchar(64)
)
----------------------------------------------------------------
--SPLIT
----------------------------------------------------------------
create procedure spInsertPostCategoryDal(
    Name nvarchar(32),
    DefinitionUniqueId varchar(64)
    ) as 
    begin
        insert into PostCategories (Name,DefinitionUniqueId) 
                values (@Name,@DefinitionUniqueId)

        declare @newId bigint=(IDENT_CURRENT('PostCategories'));
        select * from PostCategories where Id=@newId;
    end
go
----------------------------------------------------------------
create procedure spReadAllPostCategoryDals() as
begin
    
        select * from PostCategories
    end
go
----------------------------------------------------------------
create procedure spReadPostCategoryDalById(@Id bigint) as
    begin
        select * from PostCategories where Id=@Id
    end
go
----------------------------------------------------------------
create procedure spDeletePostCategoryDalById(@Id bigint) as
begin
        delete from PostCategories where Id=@Id
    end
go
----------------------------------------------------------------
        ";


            
            
            File.WriteAllText("0000-temp.sql",script);
            
            var man = new BuildupScriptManager(".");

            var sinfo = man[0];

            var parts = sinfo.SplitScriptIntoBatches();

            foreach (var part in parts)
            {
                Console.WriteLine(part);

                Console.WriteLine("<<<<<<<<<<<<<<>>>>>>>>>>>>>>");
            }


        }
    }
}