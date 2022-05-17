using System.Collections.Generic;
using Meadow.Utility;

namespace Meadow.Configuration.ConfigurationRequests
{
    public abstract class ConfigurationRequest<TResult> : MeadowRequest<MeadowVoid, TResult>
        where TResult : class, new()
    {
        protected MeadowConfiguration Configuration { get; private set; }

        protected Dictionary<string, string> ConfigurationMap { get; private set; } = new Dictionary<string, string>();


        public ConfigurationRequest(bool returnsValue) : base(returnsValue)
        {
        }

        public ConfigurationRequestResult Result { get; set; }

        public virtual MeadowConfiguration Initialize(MeadowConfiguration configuration)
        {
            Configuration = configuration;

            ConfigurationMap = new ConnectionStringParser().Parse(configuration.ConnectionString);

            configuration = ReConfigure(configuration, ConfigurationMap);

            this.RequestText = GetQuery();

            return configuration;
        }

        protected virtual MeadowConfiguration ReConfigure(MeadowConfiguration config,
            Dictionary<string, string> valuesMap)
        {
            return config;
        }

        protected abstract string GetQuery();
    }
}