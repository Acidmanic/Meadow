using Meadow.Requests.Common.Models;

namespace Meadow.Requests.Common
{
    public sealed class ReadChunkRequest<TStorage> : MeadowRequest<ChunkResponse<TStorage>>
    {
        public ReadChunkRequest(string searchId, long offset = 0, long size = 100) : 
            base(new {Offset = offset,Size = size,SearchId = searchId})
        {
        }

        public override string RequestText => Convention<TStorage>().ReadChunkProcedureName;
    }
}