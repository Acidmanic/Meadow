using Example.Filtering.Models;
using Example.Filtering.Requests.Models;
using Meadow.Requests;

namespace Example.Filtering.Requests
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