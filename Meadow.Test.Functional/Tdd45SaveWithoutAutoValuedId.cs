using System;
using System.Linq;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;
using Microsoft.Extensions.Logging;

namespace Meadow.Test.Functional
{
    public class Tdd45SaveWithoutAutoValuedId:MeadowMultiDatabaseTestBase
    {
        protected override void SelectDatabase()
        {
            UseSqLite();
        }

        protected override void Main(MeadowEngine engine, ILogger logger)
        {


            var category = new Category
            {
                Description = "This is a testy category",
                Id = Guid.NewGuid().ToString(),
                Title = "New"
            };

            var inserted = engine.PerformRequest(new SaveRequest<Category>(category))
                .FromStorage.FirstOrDefault();

            if (inserted == null)
            {
                throw new Exception("Problem saving New Category");
            }
            
            PrintObject(inserted);
            
        }
    }
}