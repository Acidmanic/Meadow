using Example.Filtering.Models;
using Example.Filtering.Requests.Models;
using Meadow.Requests;

namespace Example.Filtering.Requests
{
    public sealed class ReadPersonsChunkRequest : MeadowRequest<ChunkShell, Person>
    {
        public ReadPersonsChunkRequest(string searchId, long offset, long size) : base(true)
        {
            ToStorage = new ChunkShell
            {
                SearchId = searchId,
                Offset = offset,
                Size = size
            };
        }
    }
}