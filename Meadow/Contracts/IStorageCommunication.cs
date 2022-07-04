using System;
using Meadow.Configuration;
using Meadow.Requests;

namespace Meadow.Contracts
{
    public interface IStorageCommunication<TToStorageCarrier, out TFromStorageCarrier>
    {


        TToStorageCarrier CreateToStorageCarrier(MeadowRequest request,MeadowConfiguration configuration);


        void Communicate(TToStorageCarrier carrier,Action<TFromStorageCarrier> onDataAvailable, MeadowConfiguration configuration, bool returnsValue);

    }
}