using Meadow.BuildupScripts;
using Meadow.Configuration.ConfigurationRequests.Models;
using Meadow.Reflection.Conventions;
using Meadow.Requests;

namespace Meadow.Configuration.ConfigurationRequests
{
    public class LastExecutedHistoryRequest : MeadowRequest<MeadowVoid, MeadowDatabaseHistory>
    {
        public LastExecutedHistoryRequest() : base(true)
        {
        }


        protected override string GetRequestText()
        {
            return new NameConvention(typeof(MeadowDatabaseHistory)).SelectLastProcedureName;
        }
    }
}