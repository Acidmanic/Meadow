using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Requests.Configuration.Abstractions;
using Meadow.Utility;

namespace Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests
{
    class AdoCreateDatabaseRequest : ConfigurationRequest
    {
        private string _providedDbName = "MeadowDatabase";
        private readonly Func<string, string> _createDatabase;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDatabase">Returns the sql needed for creating a database. takes the database name as argument</param>
        public AdoCreateDatabaseRequest(Func<string, string> createDatabase)
        {
            _createDatabase = createDatabase;
        }


        public override MeadowConfiguration AlterConfiguration(MeadowConfiguration configuration,
            Dictionary<string, string> connectionString)
        {
            if (connectionString.ContainsKey("Database"))
            {
                _providedDbName = connectionString["Database"];

                connectionString.Remove("Database");
            }

            return new MeadowConfiguration()
            {
                ConnectionString = new ConnectionStringParser().CreateConnectionString(connectionString),
            };
        }

        public override string RequestText => _createDatabase(_providedDbName);
    }
}