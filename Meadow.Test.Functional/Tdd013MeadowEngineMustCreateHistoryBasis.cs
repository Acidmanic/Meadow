using System.Reflection.Metadata;
using Meadow.Sql;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Utility;

namespace Meadow.Test.Functional
{
    public class Tdd013MeadowEngineMustCreateHistoryBasis:MeadowFunctionalTest
    {


        public Tdd013MeadowEngineMustCreateHistoryBasis():base("MeadowScratch")
        {
            
        }
        
        public override void Main()
        {
            
            MeadowEngine.UseDataAccess(new CoreProvider<SqlDataAccessCore>());
            
            var engine = SetupClearDatabase(true);
        }
    }
}