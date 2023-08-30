using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests.Models;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class ReadChunkRequest<TStorage> : MeadowRequest<ChunkShell, TStorage> where TStorage : class, new()
    {
        public ReadChunkRequest(string searchId) : base(true)
        {
            ToStorage = new ChunkShell
            {
                Offset = 0,
                Size = 100,
                SearchId = searchId
            };
        }

        public override string RequestText
        {
            get => Configuration.GetNameConvention<TStorage>().ReadChunkProcedureName;
            protected set
            {
                    
            }
        }
    }
}