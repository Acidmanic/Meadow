using Meadow.Configuration;

namespace Meadow.SQLite.Extensions
{
    internal static class MeadowConfigurationsSqLiteProceduresManagerExtensions
    {

        public static SqLiteProcedureManager GetSqLiteProcedureManager(this MeadowConfiguration configurations)
        {
            return SqLiteProcedureManager.Connect(configurations.ConnectionString);
        }
        
        
    }
}