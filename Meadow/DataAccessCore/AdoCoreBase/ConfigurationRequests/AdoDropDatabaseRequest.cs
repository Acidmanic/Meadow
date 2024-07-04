using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Requests.Configuration.Abstractions;
using Meadow.Utility;

namespace Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests
{
    class AdoDropDatabaseRequest : ConfigurationRequest
    {
        private string _providedDbName = "MeadowDatabase";
        private readonly Func<string, string> _query;

        public AdoDropDatabaseRequest(Func<string, string> query)
        {
            _query = query;
        }

        public override MeadowConfiguration AlterConfiguration(MeadowConfiguration configuration, Dictionary<string, string> connectionString)
        {
            if (connectionString.ContainsKey("Database"))
            {
                _providedDbName = connectionString["Database"];

                connectionString.Remove("Database");
            }

            return new MeadowConfiguration()
            {
                ConnectionString = new ConnectionStringParser().CreateConnectionString(connectionString)
            };
        }

        public override string RequestText => _query(_providedDbName);
    }
}