using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Extensions;
using Meadow.Configuration;
using Meadow.Inclusion;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{


    public class Tdd49DevelopViews : IFunctionalTest
    {
        
        public record User(string Name, string Surname, Guid Id, Guid MasId);


        public record Shambal(Guid Id, Guid GhozmitId, Ghozmit Ghozmit, string Title, string Description);

        public record Ghozmit(Guid Id, string Name);

        public record Mas(Guid Id, List<User> Users, Shambal Shambal, Guid ShambalId, string First, string Second, int Index);

        public class CustomView : View<CustomView,Mas>
        {
            
            public Guid CurrentUserId { get; set; }
            
            protected override void MarkInclusions()
            {
                Include(m => m.Users).Where(u => u.MasId).IsEqual().To((Mas m) => m.Id);
                
                Include(m => m.Users).Where(u => u.MasId).IsEqual().To((Mas m) => m.Id)
                    .Or().Where(u => u.Id).IsEqual().To(v => v.CurrentUserId);

                Include(m => m.Shambal);

                Include(m => m.Shambal.Ghozmit);
            }
        }

        public class FullTreeMas : FullTreeView<Mas>
        {
        }

        public void Main()
        {


            // var ov = new ObjectEvaluator(typeof(Mas));
            //
            //
            // var meeh = MemberOwnerUtilities.GetKey<Mas,string>(m => m.First);
            //
            
            var v = new CustomView();

            var configuration = new MeadowConfiguration();

            Console.WriteLine(v.Script(configuration));
        }
    }
}