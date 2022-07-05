using Meadow.Contracts;
using Meadow.DataAccessCore;

namespace Meadow.Utility
{
    public class CoreProvider<TCore> : IMeadowDataAccessCoreProvider where TCore:IMeadowDataAccessCore,new()
    {
        public IMeadowDataAccessCore CreateDataAccessCore()
        {
            return new TCore();
        }
    }
}