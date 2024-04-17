using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Models;
using Meadow.Requests;
using Meadow.Requests.Configuration.Abstractions;
using Meadow.Utility;

namespace Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests
{
    class DatabaseExistsRequest : ConfigurationRequest
    {
        private string _providedDbName = "MeadowDatabase";
        private readonly Func<string, string> _databaseExistsQuery;

        // public DatabaseExistsRequest(Func<string, string> databaseExistsQuery)
        // {
        //     _databaseExistsQuery = databaseExistsQuery;
        // }

        // protected override MeadowConfiguration ReConfigure(MeadowConfiguration config,
        //     Dictionary<string, string> valuesMap)
        // {
        //     if (valuesMap.ContainsKey("Database"))
        //     {
        //         _providedDbName = valuesMap["Database"];
        //
        //         valuesMap.Remove("Database");
        //     }
        //
        //     return new MeadowConfiguration()
        //     {
        //         ConnectionString = new ConnectionStringParser().CreateConnectionString(valuesMap),
        //         BuildupScriptDirectory = config.BuildupScriptDirectory
        //     };
        // }

        // protected override string GetRequestText()
        // {
        //     return _databaseExistsQuery(_providedDbName);
        // }
        public DatabaseExistsRequest(bool returnsValue, params object[] toStorage) : base(returnsValue, toStorage)
        {
        }
    }
}