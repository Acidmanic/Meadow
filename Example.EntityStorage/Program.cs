using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Example.EntityStorage.Entities;
using Example.EntityStorage.Requests;
using Meadow;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.MySql;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.EntityStorage
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
                DatabaseFieldNameDelimiter = '_',
                MacroContainingAssemblies = new List<Assembly>
                {
                    Assembly.GetEntryAssembly(),
                    TheMeadow.Anchor.GetMeadowAssembly(),
                    TheMeadow.Anchor.GetMySqlMeadowAssembly(),
                },
                MacroPolicy = MacroPolicies.UpdateScripts
            };
            // Create Engine:
            var engine = new MeadowEngine(configuration).UseMySql();


            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }
            
            
            // Create Database if not exists
            engine.CreateIfNotExist();
            // Setup (update regarding scripts)
            engine.BuildUpDatabase();
            // ready to use

            var firstPlant = Plant.Create("Rose");
            
            var insertedFirstPlant = engine.PerformRequest(new InsertPlantRequest(firstPlant));
            
            
            var secondPlant = Plant.Create("Rose");
            
            var insertedSecondPlant = engine.PerformRequest(new InsertPlantRequest(secondPlant));
            
            // Make a request
            var request = new ReadAllPlantsRequest();

            var response = engine.PerformRequest(request);

            var allPersons = response.FromStorage;

            Console.WriteLine($"Read {allPersons.Count} Persons from database, which where inserted from scripts.");
            
            allPersons.ForEach(p=> Console.WriteLine($"--- {p.Name + " " + p.Id} Created At: {p.CreateDate}"));

            
        }
        
        /// <summary>
        /// :D
        /// </summary>
        /// <returns>Sql server password</returns>
        private static string ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt()
        {
            try
            {
                return File.ReadAllText(Path.Join("..","..","..","..", "sa-pass"));
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

            return $"Allow User Variables=True;Server=192.168.148.1;Database=MeadowScratch;Uid=remote_looser;Pwd='{password.Trim()}';";
        }
    }
}
