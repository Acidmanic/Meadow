using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Models;
using Meadow.Requests;
using Meadow.Requests.Configuration.Abstractions;
using Meadow.Utility;

namespace Meadow.MySql.ConfigurationRequests
{
    class FindDatabase : ConfigurationFunctionRequest<NameResult>
    {
        private string _providedDbName = "MeadoDatabase";


        protected override string GetRequestText()
        {
            return
                $@"SELECT SCHEMA_NAME Name FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='{_providedDbName}';";
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
                ConnectionString = new ConnectionStringParser().CreateConnectionString(valuesMap)
            };
        }

    }
}