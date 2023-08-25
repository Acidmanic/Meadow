using Example.SqLite.Filtering.Models;
using Example.SqLite.Filtering.Requests.Models;
using Meadow.Requests;

namespace Example.SqLite.Filtering.Requests
{
    public sealed class ReadPersonsChunkRequest : MeadowRequest<ChunkShell, Person>
    {
        public ReadPersonsChunkRequest(string filterHash, long offset, long size) : base(true)
        {
            ToStorage = new ChunkShell
            {
                FilterHash = filterHash,
                Offset = offset,
                Size = size
            };
        }
    }
}