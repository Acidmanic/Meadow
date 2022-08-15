using Meadow.Contracts;
using Meadow.DataAccessCore;

namespace Meadow.Test.Functional.FakeEngine
{
    public class FakeCoreProvider:IMeadowDataAccessCoreProvider
    {
        public IMeadowDataAccessCore CreateDataAccessCore()
        {
            return new FakeCore();
        }
    }
}