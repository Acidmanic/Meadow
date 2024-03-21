using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Inclusion;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd49DevelopViews : IFunctionalTest
    {
        
        public record User(string Name, string Surname, Guid Id, Guid MasId);


        public record Shambal(Guid Id, string Title, string Description);

        public record Mas(Guid Id, List<User> Users, Shambal Shambal, Guid ShambalId, string First, string Second, int Index);

        public class ViewT : View<Mas>
        {
            protected override void MarkInclusions()
            {
                Include(m => m.Users).Where(u => u.MasId).IsEqual().To((Mas m) => m.Id);

                Include(m => m.Shambal);
            }
        }
        
        public void Main()
        {


            // var ov = new ObjectEvaluator(typeof(Mas));
            //
            //
            // var meeh = MemberOwnerUtilities.GetKey<Mas,string>(m => m.First);
            //
            
            var v = new ViewT();


            var configuration = new MeadowConfiguration();

            Console.WriteLine(v.Script(configuration));
        }
    }
}