using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Models;
using Meadow.Requests;
using Meadow.Utility;

namespace Meadow.MySql.ConfigurationRequests
{
    class CreateIfNotExistRequest : ConfigurationFunctionRequest<BooleanResult>
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

            return
                $@"
                SELECT IF(count(SCHEMA_NAME)>=1,0,1) into @existance FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{_providedDbName}' ;

                CREATE DATABASE IF NOT EXISTS {_providedDbName};

                SELECT @existance Value;
                ";
        }
    }
}