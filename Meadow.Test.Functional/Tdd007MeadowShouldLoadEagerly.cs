using Meadow.Test.Functional.Requests;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd007MeadowShouldLoadEagerly:MeadowFunctionalTest
    {
        public Tdd007MeadowShouldLoadEagerly() :base("MeadowScratch"){ }
        public override void Main()
        {
            var engine = SetupClearDatabase();
            
            var request = new GetPersonByIdEager(1);

            var result = engine.PerformRequest(request);
            
            PrintObject(result.FromStorage);
        }
    }
}