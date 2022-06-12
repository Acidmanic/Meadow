using Acidmanic.Utilities.Reflection.Dynamics;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd014CreatingDynamicType : MeadowFunctionalTest
    {
        public override void Main()
        {
            var idShell = new ModelBuilder("IdShell")
                .AddProperty("Id", typeof(long))
                .AddProperty("Name",typeof(string))
                .AddProperty("User",typeof(Tag))
                    .BuildObject();
            
            PrintObject(idShell);
        }
    }
}