using System;
using System.Threading.Channels;
using Meadow.Configuration;
using Meadow.Log;

namespace Meadow.Test.Functional
{
    public class MeadowShouldBuildupTheDatabase : IFunctionalTest
    {
        private static string connectionString =
            "Server=localhost;User Id=sa; Password=never54aga.1n;Database=MeadowScratch; MultipleActiveResultSets=true";

        public void Main()
        {
            var engine = new MeadowEngine(
                new MeadowConfiguration
                {
                    ConnectionString = connectionString,
                    BuildupScriptDirectory = "Scripts"
                }, new ConsoleLogger());

            if (engine.DatabaseExists())
            {
                Console.WriteLine("The Older database was deleted");

                engine.DropDatabase();
            }

            engine.CreateDatabase();

            engine.BuildUpDatabase();
        }
    }
}