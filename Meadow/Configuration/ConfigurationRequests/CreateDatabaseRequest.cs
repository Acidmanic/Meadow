using System.Collections.Generic;
using Meadow.Utility;

namespace Meadow.Configuration.ConfigurationRequests
{
    class CreateDatabaseRequest : ConfigurationCommandRequest
    {
        private string _providedDbName = "MeadoDatabase";

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
                ConnectionString = new ConnectionStringParser().CreateConnectionString(valuesMap)
            };
        }

        protected override string GetQuery()
        {
            return $@"CREATE DATABASE {_providedDbName}";
        }
    }
}