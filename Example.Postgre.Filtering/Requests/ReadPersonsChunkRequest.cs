using Example.Postgre.Filtering.Models;
using Example.Postgre.Filtering.Requests.Models;
using Meadow.Requests;

namespace Example.Postgre.Filtering.Requests
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