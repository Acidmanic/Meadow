﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Example.Macros.MySql.Requests;
using Meadow;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.MySql;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.Macros.MySql
{
    class Program
    {

        public class MeadowConfigProvider : IMeadowConfigurationProvider
        {
            public MeadowConfiguration GetConfigurations()
            {
                return new MeadowConfiguration
                {
                    ConnectionString = GetConnectionString(),
                    BuildupScriptDirectory = "Scripts",
                    DatabaseFieldNameDelimiter = '_',
                    MacroPolicy = MacroPolicies.UpdateScripts,
                    MacroContainingAssemblies = new List<Assembly>{typeof(Program).Assembly}
                };
            }
        }
        
        static void Main(string[] args)
        {
            new ConsoleLogger().UseForMeadow();

            // Configure Meadow
            var configuration = new MeadowConfigProvider().GetConfigurations();
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
            var reachedTheEnd = false;

            var currentPath = new DirectoryInfo(".").FullName;

            while (!reachedTheEnd)
            {
                var currentFile = Path.Join(currentPath, "sa-pass");

                if (File.Exists(currentFile))
                {
                    return File.ReadAllText(currentFile);
                }

                var currentDirectory = new DirectoryInfo(currentPath);

                var parent = currentDirectory.Parent;
                
                reachedTheEnd = parent == null;

                if (!reachedTheEnd)
                {
                    currentPath = parent.FullName;
                }
            }
            throw new Exception("Please create a text file, named 'sa-pass' " +
                                "containing your password, and put it in the solution directory.");
        }

        private static string GetConnectionString()
        {
            var password = ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt();

            return $"Allow User Variables=True;Server=localhost;Database=MeadowScratch;Uid=sa;Pwd='{password.Trim()}';";
        }
    }
}