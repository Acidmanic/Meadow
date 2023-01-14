using System;
using System.IO;
using System.Threading.Tasks;
using Example.Async.SqlServer.Requests;
using Meadow;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.SqlServer;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.Async.SqlServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            new ConsoleLogger().UseForMeadow();
            // Configure Meadow
            var configuration = new MeadowConfiguration
            {
                ConnectionString = GetConnectionString(),
                BuildupScriptDirectory = "Scripts"
            };
            // Create Engine:
            var engine = new MeadowEngine(configuration).UseSqlServer();

            if (await engine.DatabaseExistsAsync())
            {
                await engine.DropDatabaseAsync();
            }
            // Create Database if not exists
            await engine.CreateIfNotExistAsync();
            // Setup (update regarding scripts)
            await engine.BuildUpDatabaseAsync();
            // ready to use
            // Make a request
            var request = new GetAllPersonsFullTreeRequest();

            var response = await engine.PerformRequestAsync(request);

            var allPersons = response.FromStorage;

            Console.WriteLine($"Read {allPersons.Count} Persons from database, which where inserted from scripts.");
            
            allPersons.ForEach(p=> Console.WriteLine($"--- {p.Name + " " + p.Surname}"));
            
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

            return "Server=localhost;" +
                   "User Id=sa; " +
                   $"Password={password};" +
                   "Database=MeadowTestDb; " +
                   "MultipleActiveResultSets=true";
        }
    }
}