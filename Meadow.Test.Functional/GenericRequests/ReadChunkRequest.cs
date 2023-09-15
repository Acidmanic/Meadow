using Meadow.Extensions;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests.Models;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.GenericRequests
{
    public sealed class ReadChunkRequest<TStorage> : MeadowRequest<ChunkShell, TStorage> where TStorage : class, new()
    {
        public ReadChunkRequest(string searchId,long offset =0,long size =100) : base(true)
        {
            ToStorage = new ChunkShell
            {
                Offset = offset,
                Size = size,
                SearchId = searchId
            };
        }

        public override string RequestText
        {
            get => FullTreeReadWrite()
                ? Configuration.GetNameConvention<TStorage>().ReadChunkProcedureNameFullTree
                : Configuration.GetNameConvention<TStorage>().ReadChunkProcedureName;
            protected set { }
        }
    }
}