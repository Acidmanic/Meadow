using System;
using Meadow.Configuration;

namespace Meadow.Test.Functional
{
    public class MeadowShouldCreateADatabase : IFunctionalTest
    {
        private static string connectionString =
            "Server=localhost;User Id=sa; Password=never54aga.1n;Database=Mexghun; MultipleActiveResultSets=true";

        public void Main()
        {
            var engine = new MeadowEngine(new MeadowConfiguration {ConnectionString = connectionString});

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