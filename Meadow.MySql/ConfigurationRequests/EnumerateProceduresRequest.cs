using Meadow.Models;
using Meadow.Requests;

namespace Meadow.MySql.ConfigurationRequests
{
    public class EnumerateProceduresRequest:ConfigurationFunctionRequest<NameResult>
    {
        protected override string GetRequestText()
        {
            return "select SPECIFIC_NAME Name from information_schema.ROUTINES where ROUTINE_SCHEMA = database();";
        }
    }
}