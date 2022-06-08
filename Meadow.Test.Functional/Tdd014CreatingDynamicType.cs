using System;
using System.Net.Mime;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Meadow.Reflection.Dynamics;
using Meadow.Reflection.ObjectTree;
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