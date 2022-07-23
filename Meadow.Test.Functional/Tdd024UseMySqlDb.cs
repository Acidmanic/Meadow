using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Configuration;
using Meadow.Log;
using Meadow.MySql;
using Meadow.Requests;
using Meadow.SqlServer;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd024UseMySqlDb:MeadowFunctionalTest
    {

        private class Person
        {
            public string Name { get; set; }
            
            public string Surname { get; set; }
            
            [AutoValuedMember]
            [UniqueMember]
            public long Id { get; set; }
        }

        private class ReadAllPersonsRequest : MeadowRequest<MeadowVoid, Person>
        {
            public ReadAllPersonsRequest() : base(true)
            {
            }
        }
        
        public override void Main()
        {
            var sqlConfig = new MeadowConfiguration
            {
                ConnectionString = "Server=localhost;Database=MeadowScratch;Uid=sa;Pwd='never54aga.1n'; ",
                BuildupScriptDirectory = "MySqlScripts"
            };
            
            var engine = new MeadowEngine(sqlConfig,new ConsoleLogger()).UseMySql();
            
            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }
            
            engine.CreateDatabase();
            
            engine.BuildUpDatabase();
            
            var request = new ReadAllPersonsRequest();

            var response = engine.PerformRequest(request);
            
            PrintObject(response.FromStorage);
        }
    }
}