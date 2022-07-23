using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Models;
using Meadow.Requests;
using Meadow.Utility;

namespace Meadow.SqlServer.ConfigurationRequests
{
    class DatabaseExistsRequest : ConfigurationFunctionRequest<BooleanResult>
    {
        private string _providedDbName = "MeadoDatabase";


        protected override string GetRequestText()
        {
            return
                $@"IF (DB_ID('{_providedDbName}') IS NOT NULL)
                    select cast(1 as bit) Value 
                ELSE 
                    select cast(0 as bit) Value";
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