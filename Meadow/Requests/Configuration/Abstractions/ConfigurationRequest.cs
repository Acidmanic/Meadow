using System.Collections.Generic;
using Meadow.Configuration;

namespace Meadow.Requests.Configuration.Abstractions
{
    public abstract class ConfigurationRequest : MeadowRequest,IConfigurationAlteringRequest
    {

        protected ConfigurationRequest(params object[] toStorage) : base(false, toStorage)
        {
            Execution = RequestExecution.RequestTextIsExecutable;
        }

        public virtual MeadowConfiguration AlterConfiguration(MeadowConfiguration configuration, Dictionary<string, string> connectionString)
        {
            return configuration;
        }
    }
    
    public abstract class ConfigurationRequest<TResult> : MeadowRequest<TResult>,IConfigurationAlteringRequest
    {
        protected ConfigurationRequest(params object[] toStorage) : base(true, toStorage)
        {
            Execution = RequestExecution.RequestTextIsExecutable;
        }

        public virtual MeadowConfiguration AlterConfiguration(MeadowConfiguration configuration, Dictionary<string, string> connectionString)
        {
            return configuration;
        }

    }
}