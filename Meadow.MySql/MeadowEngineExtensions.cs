using Meadow.Utility;

namespace Meadow.MySql
{
    public static class MeadowEngineExtensions
    {
        public static MeadowEngine UseMySql(this MeadowEngine engine)
        {

            MeadowEngine.UseDataAccess(new CoreProvider<MySqlDataAccessCore>());

            return engine;
        }
    }
}