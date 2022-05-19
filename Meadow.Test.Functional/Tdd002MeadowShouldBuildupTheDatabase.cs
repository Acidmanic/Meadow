using System;
using System.Threading.Channels;
using Meadow.Configuration;
using Meadow.Log;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd002MeadowShouldBuildupTheDatabase : MeadowFunctionalTest
    {
        public Tdd002MeadowShouldBuildupTheDatabase():base("MeadowScratch") { }
        
        public override void Main()
        {
            var engine = CreateEngine();

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