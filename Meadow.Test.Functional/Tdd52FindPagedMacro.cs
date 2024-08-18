// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Acidmanic.Utilities.Filtering;
// using Acidmanic.Utilities.Filtering.Utilities;
// using Meadow.Configuration;
// using Meadow.Test.Functional.GenericRequests;
// using Meadow.Test.Functional.Models;
// using Meadow.Test.Functional.TestEnvironment;
// using Microsoft.Extensions.Logging;
//
// namespace Meadow.Test.Functional;
//
// public class Tdd52FindPagedMacro : PersonUseCaseTestBase
// {
//     protected override void SelectDatabase()
//     {
//         UseMySql();
//     }
//
//
//     protected override MeadowConfiguration RegulateConfigurations(MeadowConfiguration configurations)
//     {
//         configurations.AddFilter<Deletable>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
//         configurations.AddFilter<Person>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
//         configurations.AddFilter<Job>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
//         configurations.AddFilter<Address>(builder => builder.Where(d => d.IsDeleted).IsEqualTo(false));
//
//
//         return configurations;
//     }
//
//
//     protected override void Main(MeadowEngine engine, ILogger logger)
//     {
//         var allPersonResponse = engine
//             .PerformRequest(new FindPagedRequest<Person>(new FilterQuery()));
//
//         if (allPersonResponse.Failed)
//         {
//             throw allPersonResponse.FailureException;
//         }
//
//         var allPersons = allPersonResponse.FromStorage;
//
//         if (allPersons.Count != Persons.Length)
//         {
//             throw new Exception("Problem with reading all records in find pages");
//         }
//         
//         Index<Person, long>(engine, Persons);
//
//         FindPagedMustFindExpectedItemsForGivenSearchTerms(engine,
//             Choose(Persons.FirstOrDefault(p => p.Name=="Farshid")),"farshid");
//         
//         FindPagedMustFindExpectedItemsForGivenSearchTerms(engine,
//             Choose(Persons.Where(p => 
//                 p.Name=="Mani" ||
//                 p.Name=="Mona" ||
//                 p.Name=="Farshid"
//                 )),"Moai");
//         
//         FindPagedMustFindExpectedItemsForGivenSearchTerms(engine,
//             Choose(Persons.Where(p => 
//                 p.Name=="Farshid" ||
//                 p.Name=="Farimehr"
//             )),"Far");
//         
//         logger.LogInformation("[PASS] Searching Is Ok");
//     }
//
//     private void FindPagedMustFindExpectedItemsForGivenSearchTerms(MeadowEngine engine, List<Person> choose, string farshid)
//     {
//         throw new NotImplementedException();
//     }
//
//     private List<T> Choose<T>(params T[] items) => new List<T>(items);
//     
//     private List<T> Choose<T>(IEnumerable<T> items) => new List<T>(items);
//     
//     private List<TModel> Sort<TModel>(IEnumerable<TModel> items, Comparison<TModel> compare)
//     {
//         var list = new List<TModel>(items);
//
//         list.Sort(compare);
//
//         return list;
//     }
//
//     private void FindPagedMustFindRecords(MeadowEngine engine, Action<FilterQueryBuilder<Person>> select,
//         params Person[] items)
//     {
//         var queryBuilder = new FilterQueryBuilder<Person>();
//
//         select(queryBuilder);
//
//         var query = queryBuilder.Build();
//
//         var found = engine
//             .PerformRequest(new FindPagedRequest<Person>(query))
//             .FromStorage;
//
//         foreach (var person in items)
//         {
//             AssertX.ContainShallow(found, person, $"Person: {person.Name} was not found in the result");
//         }
//
//         AssertX.AreSameSize(items, found, "Extra Items has been read from database");
//     }
//
//
//     private void FindPagedMustPerformCorrectSorting(MeadowEngine engine, List<Person> expected, Action<OrderSetBuilder<Person>> buildOrders)
//     {
//         var orderBuilder = new OrderSetBuilder<Person>();
//
//         buildOrders(orderBuilder);
//
//         var orders = orderBuilder.Build();
//
//         var found = engine
//             .PerformRequest(new FindPagedRequest<Person>(new FilterQuery(), 0, 1000, null, orders))
//             .FromStorage;
//
//         AssertX.AreSameSize(expected, found, "Read items does not match with expectations");
//
//         AssertX.InSameOrderShallow(expected, found);
//     }
//     
// }