using Meadow.Models;
using Meadow.Requests;
using Meadow.Requests.Configuration.Abstractions;

namespace Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests
{
    public class NameResultQuery:ConfigurationFunctionRequest<NameResult>
    {
        private readonly string _query;

        public NameResultQuery(string query)
        {
            _query = query;
        }

        protected override string GetRequestText()
        {
            return _query;
        }
    }
}