using System.Collections.Generic;
using Meadow.Configuration.ConfigurationRequests.Models;
using Meadow.Requests;

namespace Meadow.Configuration.ConfigurationRequests
{
    public class EnumerateTablesRequest:ConfigurationFunctionRequest<NameResult>
    {
        protected override string GetRequestText()
        {
            return "SELECT name 'Name' FROM sys.objects WHERE type_desc = 'USER_TABLE';";
        }
    }
}