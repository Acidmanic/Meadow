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
            public List<string> MacroContainingAssemblyFiles { get; set; } = new List<string>();
        }

        public JsonConfigurationProvider(string filePath, ILogger logger)
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