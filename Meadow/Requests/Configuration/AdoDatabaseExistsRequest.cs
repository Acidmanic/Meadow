using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests.Configuration.Abstractions;
using Meadow.Utility;

namespace Meadow.Requests.Configuration
{
    class AdoDatabaseExistsRequest : ConfigurationRequest
    {

        public override MeadowConfiguration AlterConfiguration(MeadowConfiguration configuration, Dictionary<string, string> connectionString)
        {

            var dataBaseName = "MeadowDatabase";
            
            if (connectionString.ContainsKey("Database"))
            {
                dataBaseName = connectionString["Database"];
            
                connectionString.Remove("Database");
            }
            
            SetToStorage(new {DatabaseName=dataBaseName});
            
            return new MeadowConfiguration()
            {
                ConnectionString = new ConnectionStringParser().CreateConnectionString(connectionString),
                BuildupScriptDirectory = configuration.BuildupScriptDirectory
            };
        }

        public override string RequestText => NameConvention.Reserved.DatabaseExists;
    }
}