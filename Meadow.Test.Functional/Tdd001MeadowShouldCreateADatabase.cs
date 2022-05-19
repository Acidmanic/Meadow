using System;
using Meadow.Configuration;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd001MeadowShouldCreateADatabase : MeadowFunctionalTest
    {
        
        public override void Main()
        {
            var engine = CreateEngine();

            var ex = engine.DatabaseExists();

            if (ex)
            {
                Console.WriteLine("Database was there");
                
                engine.DropDatabase();
                
                Console.WriteLine("Database Dropped");
            }
            else
            {
                Console.WriteLine("Database Not Found");
                
                engine.CreateDatabase();
                
                Console.WriteLine("Database was created");
            }
            
            //engine.CreateIfNotExist();
            
        }
    }
}