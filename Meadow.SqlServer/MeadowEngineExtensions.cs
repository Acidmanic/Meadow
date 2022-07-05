using Meadow.Utility;

namespace Meadow.SqlServer
{
    public static class MeadowEngineExtensions
    {
        public static MeadowEngine UseSqlServer(this MeadowEngine engine)
        {

            MeadowEngine.UseDataAccess(new CoreProvider<SqlDataAccessCore>());

            return engine;
        }
    }
}