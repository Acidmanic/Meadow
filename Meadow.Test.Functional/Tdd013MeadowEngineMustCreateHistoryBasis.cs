using System.Reflection.Metadata;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd013MeadowEngineMustCreateHistoryBasis:MeadowFunctionalTest
    {


        public Tdd013MeadowEngineMustCreateHistoryBasis():base("MeadowScratch")
        {
            
        }
        
        public override void Main()
        {
            var engine = SetupClearDatabase();
        }
    }
}