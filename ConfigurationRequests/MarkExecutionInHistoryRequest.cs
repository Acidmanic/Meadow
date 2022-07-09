using Meadow.BuildupScripts;
using Meadow.Configuration.ConfigurationRequests.Models;
using Meadow.Reflection.Conventions;
using Meadow.Requests.Common;

namespace Meadow.Configuration.ConfigurationRequests
{
    public class MarkExecutionInHistoryRequest : InsertSpRequest<MeadowDatabaseHistory>
    {
        public MarkExecutionInHistoryRequest(ScriptInfo script)
        {
            ToStorage = new MeadowDatabaseHistory
            {
                Script = script.Script,
                ScriptName = script.Name,
                ScriptOrder = script.OrderIndex
            };
        }

        protected override string GetRequestText()
        {
            return new NameConvention(typeof(MeadowDatabaseHistory)).InsertProcedureName;
        }
    }
}