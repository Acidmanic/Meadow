using Meadow.Configuration.ConfigurationRequests;

namespace Meadow.BuildupScripts
{
    class BuildupScriptRequest : ConfigurationCommandRequest
    {
        private readonly ScriptInfo _info;

        public BuildupScriptRequest(ScriptInfo info)
        {
            _info = info;
        }

        protected override string GetQuery()
        {
            return _info.Script;
        }
    }
}