using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Utility;

namespace Meadow.Requests
{
    public abstract class ConfigurationRequest<TResult> : MeadowRequest<TResult>
        where TResult : class
    {

        protected Dictionary<string, string> ConfigurationMap { get; private set; } = new Dictionary<string, string>();


        public ConfigurationRequest(bool returnsValue) : base(returnsValue)
        {
            Execution = RequestExecution.RequestTextIsExecutable;
        }
        
        public ConfigurationRequestResult Result { get; set; }

        public virtual MeadowConfiguration PreConfigure()
        {
            ConfigurationMap = new ConnectionStringParser().Parse(Configuration.ConnectionString);
        
            var configuration = ReConfigure(Configuration, ConfigurationMap);

            return configuration;
        }
        
        

        protected virtual MeadowConfiguration ReConfigure(MeadowConfiguration config,
            Dictionary<string, string> valuesMap)
        {
            return config;
        }

    }
}