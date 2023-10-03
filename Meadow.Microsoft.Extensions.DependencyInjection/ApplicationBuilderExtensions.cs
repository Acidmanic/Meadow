using Meadow.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Microsoft.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{


    public static IServiceProvider ConfigureMeadow(this IServiceProvider provider,Action<MeadowEngine,ILogger> configure)
    {

        var logger = provider.GetService<ILogger>() ?? new ConsoleLogger().Shorten();

        var configurationProvider = provider.GetService<IMeadowConfigurationProvider>();

        if (configurationProvider == null)
        {
            logger.LogError("Unable to get Meadow Configuration Provider, You need to register " +
                            "your IMeadowConfigurationProvider implementation on this Dependency injection container." +
                            " Configuration of Meadow has been aborted.");

            return provider;
        }
        
        var configuration = configurationProvider.GetConfigurations();

        var engine = new MeadowEngine(configuration);

        configure(engine, logger);
        
        return provider;
    }
}