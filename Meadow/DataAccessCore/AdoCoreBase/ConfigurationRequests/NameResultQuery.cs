using Meadow.Models;
using Meadow.Requests;
using Meadow.Requests.Configuration.Abstractions;

namespace Meadow.DataAccessCore.AdoCoreBase.ConfigurationRequests
{
    public class NameResultQuery:ConfigurationRequest<NameResult>
    {
        public NameResultQuery(string query)
        {
            RequestText = query;
        }

        public override string RequestText { get; }
    }
}