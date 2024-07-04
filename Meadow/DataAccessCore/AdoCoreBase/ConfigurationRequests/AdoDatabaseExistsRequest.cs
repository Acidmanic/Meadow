using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests.Configuration.Abstractions;
using Meadow.Utility;

namespace Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests
{
    record DatabaseExistsResult(bool Value);
    class AdoDatabaseExistsRequest : ConfigurationRequest<DatabaseExistsResult>
    {
        private readonly Func<string, string> _databaseExistsQuery;
        private string _providedDbName="MeadowDatabase";

        public AdoDatabaseExistsRequest(Func<string, string> databaseExistsQuery)
        {
            _databaseExistsQuery = databaseExistsQuery;
        }

        public override MeadowConfiguration AlterConfiguration(MeadowConfiguration configuration, Dictionary<string, string> connectionString)
        {

            _providedDbName = "MeadowDatabase";
            
            if (connectionString.ContainsKey("Database"))
            {
                _providedDbName = connectionString["Database"];
            
                connectionString.Remove("Database");
            }
            
            return new MeadowConfiguration()
            {
                ConnectionString = new ConnectionStringParser().CreateConnectionString(connectionString),
                BuildupScriptDirectory = configuration.BuildupScriptDirectory
            };
        }

        public override string RequestText => _databaseExistsQuery(_providedDbName);
    }
}