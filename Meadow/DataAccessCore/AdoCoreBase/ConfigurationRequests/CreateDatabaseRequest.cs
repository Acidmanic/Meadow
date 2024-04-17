using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Requests.Configuration.Abstractions;
using Meadow.Utility;

namespace Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests
{
    class CreateDatabaseRequest : ConfigurationRequest
    {
        private string _providedDbName = "MeadowDatabase";
        private readonly Func<string, string> _createDatabase;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDatabase">Returns the sql needed for creating a database. takes the database name as argument</param>
        public CreateDatabaseRequest(Func<string, string> createDatabase)
        {
            _createDatabase = createDatabase;
        }

        protected override MeadowConfiguration ReConfigure(MeadowConfiguration config,
            Dictionary<string, string> valuesMap)
        {
            if (valuesMap.ContainsKey("Database"))
            {
                _providedDbName = valuesMap["Database"];

                valuesMap.Remove("Database");
            }

            return new MeadowConfiguration()
            {
                ConnectionString = new ConnectionStringParser().CreateConnectionString(valuesMap),
                BuildupScriptDirectory = config.BuildupScriptDirectory
            };
        }

        protected override string GetRequestText()
        {
            return _createDatabase(_providedDbName);
        }
    }
}