using System.Collections.Generic;
using Meadow.Models;
using Meadow.Requests;

namespace Meadow.SQLite.Requests
{
    public class EnumerateTablesRequest : ConfigurationFunctionRequest<NameResult>
    {
        protected override string GetRequestText()
        {
            return "SELECT name 'Name' FROM sqlite_master WHERE type='table';";
        }
    }
}