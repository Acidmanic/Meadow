namespace Meadow.Requests
{
    public class SqlRequest : ConfigurationCommandRequest
    {
        public SqlRequest(string sql)
        {
            RequestText = sql;
        }

        public override string RequestText { get; }
    }
}