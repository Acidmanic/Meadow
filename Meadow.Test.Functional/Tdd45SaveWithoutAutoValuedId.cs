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

            var mashti = new Person
            {
                Name = "Mashti",
                Surname = "Dashti",
                Age = 83,
                JobId = 1
            };

            var savedCategory = engine.PerformRequest(new SaveRequest<Category>(category))
                .FromStorage.FirstOrDefault();

            if (savedCategory == null)
            {
                throw new Exception("Problem saving just-unique-id entity");
            }

            CompareEntities(category,savedCategory);
            
            PrintObject(savedCategory);
            
            var savedPerson = engine.PerformRequest(new SaveRequest<Person>(mashti))
                .FromStorage.FirstOrDefault();
            
            if (savedPerson == null)
            {
                throw new Exception("Problem saving auto-values-id entity");
            }
            
            CompareEntities(mashti,savedPerson);
            
            PrintObject(savedPerson);
            
            logger.LogInformation("[PASS] Save new object Pass");

            var repeatedCategory = engine.PerformRequest(new SaveRequest<Category>(savedCategory))
                .FromStorage.FirstOrDefault();


            var repetition = engine.PerformRequest(new ReadAllRequest<Category>())
                .FromStorage.Count(c => c.Title == category.Title);

            if (repetition != 1)
            {
                throw new Exception("Save inserted more or less");
            }
            
            CompareEntities(category,repeatedCategory);

            var repeatedPerson = engine.PerformRequest(new SaveRequest<Person>(savedPerson))
                .FromStorage.FirstOrDefault();
            
            CompareEntities(mashti,repeatedPerson);
            
            repetition = engine.PerformRequest(new ReadAllRequest<Person>())
                .FromStorage.Count(p => p.Name == mashti.Name);

            if (repetition != 1)
            {
                throw new Exception("Save inserted more or less");
            }
            
            logger.LogInformation("[PASS] Save Existing object Pass");
            
            logger.LogInformation("[PASS] Save procedure for {DbName} Is OK",DatabaseName);
            
            
        }
    }
}