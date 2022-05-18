using Meadow.Configuration.ConfigurationRequests;
using Meadow.Requests;

namespace Meadow.BuildupScripts
{
    class SqlRequest : ConfigurationCommandRequest
    {
        private readonly string _sql;

        public SqlRequest(string sql)
        {
            _sql = sql;
        }

        protected override string GetRequestText()
        {
            return _sql;
        }
    }
}