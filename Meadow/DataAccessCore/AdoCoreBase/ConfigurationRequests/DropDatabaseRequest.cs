using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Requests;
using Meadow.Utility;

namespace Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests
{
    class DropDatabaseRequest : ConfigurationCommandRequest
    {
        private string _providedDbName = "MeadowDatabase";
        private readonly Func<string, string> _query;

        public DropDatabaseRequest(Func<string, string> query)
        {
            _query = query;
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

        protected override string GetRequestText()
        {
            return _query(_providedDbName);
        }
    }
}