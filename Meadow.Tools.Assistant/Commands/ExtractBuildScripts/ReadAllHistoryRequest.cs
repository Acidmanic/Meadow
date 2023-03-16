using Meadow.Contracts;
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
            get => new NameConvention(typeof(MeadowDatabaseHistory)).SelectAllProcedureName;
            protected set { }
        }
    }
}