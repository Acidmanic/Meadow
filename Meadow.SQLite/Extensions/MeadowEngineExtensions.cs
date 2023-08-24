using Meadow.Utility;

namespace Meadow.SQLite.Extensions
{
    public static class MeadowEngineExtensions
    {
        public static MeadowEngine UseSqLite(this MeadowEngine engine)
        {

            MeadowEngine.UseDataAccess(new CoreProvider<SqLiteDataAccessCore>());

            return engine;
        }
    }
}