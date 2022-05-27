using Meadow.Configuration;

namespace Meadow.Scaffolding.Contracts
{
    public interface IMeadowConfigurationProvider
    {
        MeadowConfiguration GetConfigurations();
    }
}