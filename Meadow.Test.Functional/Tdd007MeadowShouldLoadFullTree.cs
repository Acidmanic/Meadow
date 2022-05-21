using Meadow.Test.Functional.Requests;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd007MeadowShouldLoadFullTree:MeadowFunctionalTest
    {
        public Tdd007MeadowShouldLoadFullTree() :base("MeadowScratch"){ }
        public override void Main()
        {
            var engine = SetupClearDatabase();
            
            var request = new GetAllPersonsFullTree();

            var result = engine.PerformRequest(request);
            
            PrintObject(result.FromStorage);
            
        }
    }
}