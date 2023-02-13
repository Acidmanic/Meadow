using Meadow.Configuration;

namespace Meadow.Contracts
{
    public interface IMeadowConfigurationProvider
    {
        MeadowConfiguration GetConfigurations();
    }
}