using Meadow.Utility;

namespace Meadow.SQLite
{
    public static class MeadowEngineExtensions
    {
        public static MeadowEngine UseSQLite(this MeadowEngine engine)
        {

            MeadowEngine.UseDataAccess(new CoreProvider<SQLiteDataAccessCore>());

            return engine;
        }
    }
}