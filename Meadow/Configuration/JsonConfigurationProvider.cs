using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Meadow.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Meadow.Configuration
{
    public class JsonConfigurationProvider : IMeadowConfigurationProvider
    {
        private readonly string _filePath;

        public ILogger Logger { get; }

        private class MeadowConfigurationData : MeadowConfigurationModel
        {
            public List<string> MacroContainingAssemblyFiles { get; } = new List<string>();
        }


        public static readonly string JsonFileName = "Meadow.Configurations.json";


        public JsonConfigurationProvider(ILogger logger) : this(GetExecutionDirectory(), logger)
        {
        }

        private static string GetExecutionDirectory()
        {
            var directory = ".";

            var assembly = Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                var execDir = new FileInfo(assembly.Location).Directory?.FullName;

                directory = execDir ?? new DirectoryInfo(directory).FullName;
            }

            return directory;
        }

        public JsonConfigurationProvider(string workingDirectory, ILogger logger)
            : this(logger, Path.Combine(workingDirectory, JsonFileName))
        {
        }

        protected JsonConfigurationProvider(ILogger logger, string filePath)
        {
            _filePath = filePath;
            Logger = logger;
        }

        public MeadowConfiguration GetConfigurations()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);

                var data = JsonConvert.DeserializeObject<MeadowConfigurationData>(json);

                if (data != null)
                {
                    var configuration = new MeadowConfiguration
                    {
                        ConnectionString = data.ConnectionString,
                        MacroPolicy = data.MacroPolicy,
                        BuildupScriptDirectory = data.BuildupScriptDirectory,
                        MacroContainingAssemblies = new List<Assembly>()
                    };

                    foreach (var assemblyPath in data.MacroContainingAssemblyFiles)
                    {
                        try
                        {
                            var assembly = Assembly.LoadFile(assemblyPath);

                            configuration.MacroContainingAssemblies.Add(assembly);
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning("Skipped loading assembly file: {Assembly} due " +
                                              "to exception: {Exception}", assemblyPath, e);
                        }
                    }

                    return configuration;
                }
            }

            return null;
        }
    }
}