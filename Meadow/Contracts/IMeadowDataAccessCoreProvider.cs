using Meadow.DataAccessCore;

namespace Meadow.Contracts
{
    public interface IMeadowDataAccessCoreProvider
    {
        IMeadowDataAccessCore CreateDataAccessCore();
    }
}