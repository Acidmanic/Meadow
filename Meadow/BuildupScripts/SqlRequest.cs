using Meadow.Configuration.ConfigurationRequests;

namespace Meadow.BuildupScripts
{
    class SqlRequest : ConfigurationCommandRequest
    {
        private readonly string _sql;

        public SqlRequest(string sql)
        {
            _sql = sql;
        }

        protected override string GetQuery()
        {
            return _sql;
        }
    }
}