namespace Meadow.Configuration.ConfigurationRequests
{
    public abstract class ConfigurationCommandRequest : ConfigurationRequest<MeadowVoid>
    {
        public ConfigurationCommandRequest() : base(false)
        {
        }
    }
}