using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.DataSource;
using Meadow.Extensions;
using Meadow.SqlServer;
using Meadow.Test.Functional.Models.BugCase;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Test.Functional.TestDoubles;
using Meadow.Tools.Assistant;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd018LbTestCase : MeadowFunctionalTest
    {
        public override void Main()
        {
            var dataReader = new InMemoryDataReader();

            var testcaseData = this.ReadJsonBesideAssembly<List<List<DataPoint>>>("testcase.json");

            dataReader.InsertData(testcaseData);

            var configuration = new MeadowConfiguration
            {
                TableNameProvider = new PluralDataOwnerNameProvider(),
                DatabaseFieldNameDelimiter = '.',
                UsesLegacyConditionalStandardRelationalMapping = true
            };
            
            var adapter = new SqlDataStorageAdapter(configuration, new ConsoleLogger());

            var manipulator = new FiledManipulationMarker();

            var data = adapter.ReadFromStorage<ProductClassDal>(dataReader, manipulator, true);

            PrintObject(data);
        }
    }
}