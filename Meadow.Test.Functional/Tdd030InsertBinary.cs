using System;
using System.Data.SqlClient;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Requests;
using Meadow.SqlServer;
using Meadow.Test.Functional.TDDAbstractions;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd030InsertBinary : MeadowFunctionalTest
    {

        class Model
        {
            
            [UniqueMember]
            public string Id { get; set; }
            
            [TreatAsLeaf]
            public byte[] Data { get; set; }
        }

        class InsertRequest : MeadowRequest<Model, Model>
        {
            public InsertRequest() : base(true)
            {
            }
        }
        public override void Main()
        {

            var adapter = new SqlDataStorageAdapter('_', new PluralDataOwnerNameProvider(), new ConsoleLogger());

            var filed = new FiledManipulationMarker<Model>();


            var model = new Model
            {
                Data = new byte[] { 10, 20, 3, 15 },
                Id = "unique-guid-ex"
            };

            var evaluator = new ObjectEvaluator(model);
            
            var command = new SqlCommand("spInsertModel");
            
            adapter.WriteToStorage(command,filed,evaluator);
        }
    }
}