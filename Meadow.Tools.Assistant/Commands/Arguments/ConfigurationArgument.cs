using System;
using System.IO;
using Acidmanic.Utilities.Results;
using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using Meadow.Configuration;
using Microsoft.Extensions.Logging;

namespace Meadow.Tools.Assistant.Commands.Arguments
{
    [CommandName("configuration", "c")]
    public class ConfigurationArgument : ParameterCommandBase
    {
        public static string Key => nameof(ConfigurationArgument);

        protected override void RetrieveData(Context context, string parameterStringValue)
        {
            if (File.Exists(parameterStringValue))
            {
                try
                {
                    var provider = new JsonConfigurationProvider(parameterStringValue, Logger);

                    var configuration = provider.GetConfigurations();

                    if (configuration != null)
                    {
                        context.Set(Key, new Result<MeadowConfiguration>(true, configuration));

                        return;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Problem reading given meadow configuration json file: {E}", e);
                }
            }

            context.Set(Key, new Result<MeadowConfiguration>().FailAndDefaultValue());
        }

        public override string Description => " a json file containing meadow configuration.";
    }
    
    public static class ContextConfigurationArgumentExtensions{

        public static Result<MeadowConfiguration> GetMeadowJsonConfigurations(this Context context)
        {
            return context.Get(ConfigurationArgument.Key, new Result<MeadowConfiguration>().FailAndDefaultValue());
        }
    }
}