using System.IO.Compression;
using Acidmanic.Utilities.Extensions;
using Meadow.BuildupScripts;
using Meadow.Extensions;
using Meadow.Models;
using Meadow.Requests.Common;

namespace Meadow.Configuration.Requests
{
    public sealed class MarkExecutionInHistoryRequest : InsertRequest<MeadowDatabaseHistory>
    {
        public MarkExecutionInHistoryRequest(ScriptInfo script):base(CreateHistory(script)) 
        { }

        private static MeadowDatabaseHistory CreateHistory(ScriptInfo script)
        {
            return new MeadowDatabaseHistory
            {
                Script = script.Script.CompressAsync(Compressions.GZip, CompressionLevel.Optimal).Result,
                ScriptName = script.Name,
                ScriptOrder = script.OrderIndex
            };
        }


        public override string RequestText => Configuration.GetNameConvention<MeadowDatabaseHistory>().InsertProcedureName;
    }
}