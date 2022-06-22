using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Test.Functional.Models.BugCase;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd016ProducClassDalAccessNode:MeadowFunctionalTest
    {
        public override void Main()
        {
            var node = ObjectStructure.CreateStructure<ProductClassDal>(true);
            
            new AccessNodePrinter().Print(node);
        }
    }
}