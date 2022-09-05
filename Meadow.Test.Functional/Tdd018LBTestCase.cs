using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.DataSource;
using Meadow.Extensions;
using Meadow.Requests.FieldInclusion;
using Meadow.Sql;
using Meadow.SqlServer;
using Meadow.Test.Functional.Models.BugCase;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Test.Functional.TestDoubles;

namespace Meadow.Test.Functional
{
    public class Tdd018LbTestCase : MeadowFunctionalTest
    {
        public override void Main()
        {
            var dataReader = new InMemoryDataReader();

            var testcaseData = this.ReadJsonBesideAssembly<List<List<DataPoint>>>("testcase.json");

            dataReader.InsertData(testcaseData);

            var adapter = new SqlDataStorageAdapter('.',new PluralDataOwnerNameProvider());

            var manipulator = new FiledManipulationMarker<ProductClassDal>(new PluralDataOwnerNameProvider(),false);

            var data = adapter.ReadFromStorage<ProductClassDal>(dataReader, manipulator,true);

            PrintObject(data);
        }
    }
}