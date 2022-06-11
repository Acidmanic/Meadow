using Meadow.Reflection.FetchPlug;
using Meadow.Reflection.ObjectTree;
using Meadow.Test.Functional.Models.BugCase;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd016ProducClassDalAccessNode:MeadowFunctionalTest
    {
        public override void Main()
        {
            var node = new TypeAnalyzer().ToAccessNode<ProductClassDal>(true);
            
            new AccessNodePrinter().Print(node);
        }
    }
}