using Meadow.Requests.Configuration.Abstractions;

namespace Meadow.Requests
{
    public class SqlCommandRequest : ConfigurationRequest
    {
        public SqlCommandRequest(string sql):base(false)
        {
            RequestText = sql;
        }

        public override string RequestText { get; }
    }
    
    public class SqlQueryRequest<TResult> : ConfigurationRequest<TResult>
    {
        public SqlQueryRequest(string sql):base(true)
        {
            RequestText = sql;
        }

        public override string RequestText { get; }
    }
}