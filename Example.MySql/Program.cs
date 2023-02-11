using System;
using System.IO;
using Example.MySql.Requests;
using Meadow;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.MySql;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.MySql
{
    class Program
    {
        static void Main(string[] args)
        {
            new ConsoleLogger().UseForMeadow();

            // Configure Meadow
            var configuration = new MeadowConfiguration
            {
                ConnectionString = GetConnectionString(),
                BuildupScriptDirectory = "Scripts",
                DatabaseFieldNameDelimiter = '_'
            };
            // Create Engine:
            var engine = new MeadowEngine(configuration).UseMySql();

            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }

            
            // Create Database if not exists
            if (!engine.DatabaseExists())
            {
                engine.CreateDatabase();
            }
            // // Create Database if not exists
            // engine.CreateIfNotExist();
            
            
            // Setup (update regarding scripts)
            engine.BuildUpDatabase();
            // ready to use
            // Make a request
            var request = new GetAllPersonsFullTreeRequest();

            var response = engine.PerformRequest(request);

            var allPersons = response.FromStorage;

            Console.WriteLine($"Read {allPersons.Count} Persons from database, which where inserted from scripts.");

            allPersons.ForEach(p => Console.WriteLine($"--- {p.Name + " " + p.Surname}"));
        }

        /// <summary>
        /// :D
        /// </summary>
        /// <returns>Sql server password</returns>
        private static string ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt()
        {
            try
            {
                return File.ReadAllText(Path.Join("..", "..", "..", "..", "sa-pass"));
            }
            catch (Exception e)
            {
                throw new Exception("Please create a text file, named 'sa-pass' " +
                                    "containing your password, and put it in the solution directory.");
            }
        }

        private static string GetConnectionString()
        {
            var password = ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt();

            return $"Allow User Variables=True;Server=localhost;Database=MeadowScratch;Uid=sa;Pwd='{password.Trim()}';";
        }
    }
}