using Meadow.Utility;

namespace Meadow.Postgre
{
    public static class MeadowEngineExtensions
    {

        public static MeadowEngine UsePostgre(this MeadowEngine engine)
        {
            MeadowEngine.UseDataAccess(new CoreProvider<PostgreDataAccessCore>());

            return engine;
        }
    }
}