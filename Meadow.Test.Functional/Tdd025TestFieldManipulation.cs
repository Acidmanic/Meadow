using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.DataAccessCore;
using Meadow.Log;
using Meadow.MySql;
using Meadow.Requests;
using Meadow.Requests.FieldInclusion;
using Meadow.SqlServer;
using Meadow.Test.Functional.FakeEngine;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd025TestFieldManipulation : MeadowFunctionalTest
    {
        private class A
        {
            public int Id { get; set; }

            public B B { get; set; }
        }

        private class B
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
        
        private class Person
        {
            public string Name { get; set; }

            public string Surname { get; set; }

            [AutoValuedMember] [UniqueMember] public long Id { get; set; }
            
            public A A { get; set; }
        }

        private class PrintManipulationsRequest : MeadowRequest<Person,MeadowVoid >
        {
            public PrintManipulationsRequest() : base(true)
            {
            }

            protected override bool FullTreeReadWrite()
            {
                return true;
            }

            protected override void OnFieldManipulation(IFieldInclusionMarker<Person> toStorage, IFieldInclusionMarker<MeadowVoid> fromStorage)
            {
                toStorage.Exclude(p => p.Id);

                toStorage.Exclude(p => p.Name);
                toStorage.Exclude(p => p.A.Id);
                toStorage.Exclude(p => p.A.B.Name);


                Manipulator = toStorage;
                Console.WriteLine("Happened");
            }
            
            public object Manipulator { get; private set; }

        }

        public override void Main()
        {
            
            var request = new PrintManipulationsRequest();
            
            MeadowEngine.UseDataAccess(new FakeCoreProvider());
            
            var engine = new MeadowEngine(new MeadowConfiguration());

            try
            {
                engine.PerformRequest(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            PrintObject(request.Manipulator);
        }
    }
}