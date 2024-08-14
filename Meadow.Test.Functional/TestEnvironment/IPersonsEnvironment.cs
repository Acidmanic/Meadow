using System;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.TestEnvironment;

public interface IPersonsEnvironment
{
    string[] Transliterate(object searchTerms);
    MeadowEngine Engine { get;  }

    FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>> filter = null, int offset = 0, int size = 1000, Action<OrderSetBuilder<TModel>> order = null, params string[] searchTerms) where TModel : class;

    Person[] GetPersons(Func<Person, bool> predicate);
}