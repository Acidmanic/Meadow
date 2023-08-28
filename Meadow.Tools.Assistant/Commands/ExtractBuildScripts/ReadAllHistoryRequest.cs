using Meadow.Extensions;
using Meadow.Models;
using Meadow.Requests;

namespace Meadow.Tools.Assistant.Commands.ExtractBuildScripts
{
    public class ReadAllHistoryRequest : MeadowRequest<MeadowVoid, MeadowDatabaseHistory>
    {
        public ReadAllHistoryRequest() : base(true)
        {
        }


        public override string RequestText
        {
            get => Configuration.GetNameConvention<MeadowDatabaseHistory>().SelectAllProcedureName;
            protected set { }
        }
    }
}