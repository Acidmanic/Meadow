using System;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Requests;

namespace Meadow.Test.Functional.FakeEngine
{
    public class FakeCommunication:IStorageCommunication<int, int>
    {
        public int CreateToStorageCarrier(MeadowRequest request, MeadowConfiguration configuration)
        {
            return 0;
        }

        public void Communicate(int carrier, Action<int> onDataAvailable, MeadowConfiguration configuration, bool returnsValue)
        {
            
        }
    }
}