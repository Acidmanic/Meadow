using System.IO;
using Meadow.Configuration;
using Meadow.Log;
using Meadow.SQLite;
using Meadow.SqlServer;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd020WorkWithBothDataAccesses : MeadowFunctionalTest
    {
        public override void Main()
        {
            var sqlConfig = new MeadowConfiguration
            {
                ConnectionString = GetConnectionString(),
                BuildupScriptDirectory = "Scripts"
            };
            
            var engine = new MeadowEngine(sqlConfig,new ConsoleLogger()).UseSqlServer();
            
            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }
            
            engine.CreateDatabase();
            
            engine.BuildUpDatabase();

            // var file = $"/home/diego/MeadowScratch.db";
            //
            // if (File.Exists(file))
            // {
            //     File.Delete(file);
            // }
            //
            // var sqliteConfig = new MeadowConfiguration
            // {
            //     ConnectionString = $"Data Source={file}",
            //     BuildupScriptDirectory = "Scripts"
            // };
            //
            // var engine = new MeadowEngine(sqliteConfig,new ConsoleLogger()).UseSqlServer();
            //
            // if (engine.DatabaseExists())
            // {
            //     engine.DropDatabase();
            // }
            //
            // engine.CreateDatabase();
            //
            // engine.BuildUpDatabase();
        }
    }
}