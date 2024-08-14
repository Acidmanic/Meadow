using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.TestEnvironment;

public interface IPersonsEnvironment
{
    string[] Transliterate(params string[] searchTerms);
    MeadowEngine Engine { get;  }

    FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>> filter = null, int offset = 0, int size = 1000, Action<OrderSetBuilder<TModel>> order = null, params string[] searchTerms) where TModel : class;

    Person[] GetPersons(Func<Person, bool> predicate);
    List<Person> GetSorted(Comparison<Person> compare);

    void Index<TModel>(IEnumerable<TModel> items);


    List<TModel> Update<TModel>(Func<TModel,bool> predicate, Action<TModel> update) where TModel : class, new();

}