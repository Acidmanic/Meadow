using Meadow.Configuration;
using Meadow.Log;
using Meadow.MySql;
using Meadow.SqlServer;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd024UseMySqlDb:MeadowFunctionalTest
    {
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
        }
    }
}