using System;
using System.Data.SqlClient;
using System.IO;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.BuildupScripts;
using Meadow.Configuration;
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

            var script = @"





create table Posts(
    Id bigint primary key not null identity (1,1),
    Title nvarchar(128),
    Descriptions nvarchar(512),
    DefinitionUniqueId varchar(64),
    PostDate varchar(64)
)

--SPLIT

create procedure spInsertPost(@Title nvarchar(128),
    @Descriptions nvarchar(512),
    @DefinitionUniqueId varchar(64),
    @PostDate varchar(64))
as
    insert into Posts (Title, Descriptions,DefinitionUniqueId,PostDate) values (@Title,@Descriptions,@DefinitionUniqueId,@PostDate)
    DECLARE @newId bigint=(IDENT_CURRENT('Posts'));
    SELECT * FROM Posts WHERE Id=@newId;
go

create procedure spReadPostById(@Id bigint) as
    select * from Posts where Posts.Id=@Id
go

 

create procedure spReadAllPosts as 
    
    select * from Posts;    
go


create procedure spSavePost(@Id bigint,@Title nvarchar(128),
        @Descriptions nvarchar(512),@DefinitionUniqueId varchar(64),
        @PostDate varchar(64)) as

    IF EXISTS (SELECT 1 FROM Posts WHERE DefinitionUniqueId like @DefinitionUniqueId)
        BEGIN
            update Posts set Title=@Title, Descriptions=@Descriptions
                             where DefinitionUniqueId like @DefinitionUniqueId
            SELECT * FROM Posts WHERE DefinitionUniqueId like @DefinitionUniqueId
        END
    ELSE
        BEGIN
            insert into Posts (Title, Descriptions,DefinitionUniqueId,PostDate)
            values (@Title,@Descriptions,@DefinitionUniqueId,@PostDate)
            DECLARE @newId bigint=(IDENT_CURRENT('Posts'));
            SELECT * FROM Posts WHERE Id=@newId;
        END
go";


            
            
            File.WriteAllText("0000-temp.sql",script);
            
            var man = new BuildupScriptManager(".",new MeadowConfiguration{MacroPolicy = MacroPolicies.Ignore});

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