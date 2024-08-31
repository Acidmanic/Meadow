namespace Meadow.Requests
{
    public abstract class ConfigurationFunctionRequest<TResult> : ConfigurationRequest<TResult>
        where TResult : class
    {
        public ConfigurationFunctionRequest() : base(true)
        {
        }
    }
}