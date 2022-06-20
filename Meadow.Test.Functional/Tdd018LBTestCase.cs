using System.Collections.Generic;
using Meadow.DataSource;
using Meadow.Extensions;
using Meadow.Requests.FieldManipulation;
using Meadow.Sql;
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

            var adapter = new SqlDataStorageAdapter();

            var data = adapter.ReadFromStorage<ProductClassDal>(dataReader, new FiledManipulationMarker<ProductClassDal>());

            PrintObject(data);
        }
    }
}