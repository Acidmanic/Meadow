using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;

namespace Meadow.Contracts
{
    public interface ICarrierInterceptor<TToStorageCarrier, TFromStorageCarrier>
    {
        void InterceptBeforeCommunication(TToStorageCarrier carrier, List<ObjectEvaluator> data,
            MeadowConfiguration configuration);

        void InterceptAfterCommunication(TFromStorageCarrier carrier, MeadowConfiguration configuration);
    }
}