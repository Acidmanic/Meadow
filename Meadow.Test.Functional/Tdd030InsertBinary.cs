using System;
using System.Data.SqlClient;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
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

            var configuration = new MeadowConfiguration
            {
                TableNameProvider = new PluralDataOwnerNameProvider(),
                DatabaseFieldNameDelimiter = '_',
                UsesLegacyConditionalStandardRelationalMapping = true
            };
            
            var adapter = new SqlDataStorageAdapter(configuration, new ConsoleLogger());

            var filed = new FiledManipulationMarker();


            var model = new Model
            {
                Data = new byte[] { 10, 20, 3, 15 },
                Id = "unique-guid-ex"
            };

            var evaluator = new ObjectEvaluator(model);
            
            var command = new SqlCommand("spInsertModel");
            
            adapter.WriteToStorage<Model>(command,filed,evaluator);
        }
    }
}