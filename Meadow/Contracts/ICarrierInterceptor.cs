using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.Contracts
{
    public interface ICarrierInterceptor<TToStorageCarrier,TFromStorageCarrier>
    {
      
        void InterceptBeforeCommunication(TToStorageCarrier carrier,ObjectEvaluator data);
        
        void InterceptAfterCommunication(TFromStorageCarrier carrier);
    }
}