using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Utility;

namespace Meadow.Requests
{
    public abstract class ConfigurationRequest<TResult> : MeadowRequest<MeadowVoid, TResult>
        where TResult : class, new()
    {
        protected MeadowConfiguration Configuration { get; private set; }

        protected Dictionary<string, string> ConfigurationMap { get; private set; } = new Dictionary<string, string>();


        public ConfigurationRequest(bool returnsValue) : base(returnsValue)
        {
            Execution = RequestExecution.RequestTextIsExecutable;
        }

        protected abstract string GetRequestText();
        
        public override string RequestText
        {
            get { return GetRequestText(); }
            protected set { }
        }

        public ConfigurationRequestResult Result { get; set; }

        public virtual MeadowConfiguration PreConfigure(MeadowConfiguration configuration)
        {
            Configuration = configuration;
        
            ConfigurationMap = new ConnectionStringParser().Parse(configuration.ConnectionString);
        
            configuration = ReConfigure(configuration, ConfigurationMap);

            return configuration;
        }
        
        

        protected virtual MeadowConfiguration ReConfigure(MeadowConfiguration config,
            Dictionary<string, string> valuesMap)
        {
            return config;
        }

    }
}