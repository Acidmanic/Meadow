using System.Collections.Generic;
using Meadow.Requests;
using Meadow.Utility;

namespace Meadow.Configuration.ConfigurationRequests
{
    class DropDatabaseRequest : ConfigurationCommandRequest
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

        protected override string GetRequestText()
        {
            return $@"DROP DATABASE {_providedDbName}";
        }
    }
}