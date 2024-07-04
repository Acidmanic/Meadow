using System.IO.Compression;
using Acidmanic.Utilities.Extensions;
using Meadow.BuildupScripts;
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
                Script = script.Script.CompressB64Async(Compressions.GZip, CompressionLevel.Optimal).Result,
                ScriptName = script.Name,
                ScriptOrder = script.OrderIndex
            };
        }


        public override string RequestText
        {
            get => Configuration.GetNameConvention<MeadowDatabaseHistory>().InsertProcedureName;
            protected set { }
        }
    }
}