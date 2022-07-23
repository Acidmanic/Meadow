using Meadow.Models;
using Meadow.Requests;

namespace Meadow.MySql.ConfigurationRequests
{
    public class EnumerateTablesRequest : ConfigurationFunctionRequest<NameResult>
    {
        protected override string GetRequestText()
        {
            return "select TABLE_NAME Name from information_schema.TABLES where TABLE_SCHEMA = database();";
        }
    }
}