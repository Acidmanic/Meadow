using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace Meadow.Contracts
{
    public interface ICarrierInterceptor<TToStorageCarrier,TFromStorageCarrier>
    {
      
        void InterceptBeforeCommunication(TToStorageCarrier carrier,ObjectEvaluator data);
        
        void InterceptAfterCommunication(TFromStorageCarrier carrier);
    }
}