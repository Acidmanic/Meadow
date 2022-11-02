using Meadow.Extensions;
using Meadow.Test.Functional.TDDAbstractions;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd34UnClosedConnections:MeadowFunctionalTest
    {
        
        public override void Main()
        {
            new ConsoleLogger().EnableAll().UseForMeadow();
            
            var count = 10;

            for (int i = 0; i < count; i++)
            {
                PerformDatabaseConfig();
            }
        }

        private void PerformDatabaseConfig()
        {
            var engine = SetupClearDatabase();
        }
    }
}