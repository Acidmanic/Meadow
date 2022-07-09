namespace Meadow.Requests
{
    public class SqlRequest : ConfigurationCommandRequest
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