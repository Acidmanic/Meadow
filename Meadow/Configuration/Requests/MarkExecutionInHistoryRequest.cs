using System;
using Meadow.BuildupScripts;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Models;
using Meadow.Requests.Common;

namespace Meadow.Configuration.Requests
{
    public sealed class MarkExecutionInHistoryRequest : InsertSpRequest<MeadowDatabaseHistory>
    {
        public MarkExecutionInHistoryRequest(ScriptInfo script)
        {
            ToStorage = new MeadowDatabaseHistory
            {
                Script = script.Script.ToBase64String(),
                ScriptName = script.Name,
                ScriptOrder = script.OrderIndex
            };
        }


        public override string RequestText
        {
            get { return new NameConvention(typeof(MeadowDatabaseHistory)).InsertProcedureName; }
            protected set { }
        }
    }
}