namespace Meadow.Configuration.ConfigurationRequests
{
    public abstract class ConfigurationFunctionRequest<TResult> : ConfigurationRequest<TResult>
        where TResult : class, new()
    {
        public ConfigurationFunctionRequest() : base(true)
        {
        }
    }
}