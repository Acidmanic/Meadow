using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Configuration;
using Meadow.Requests;
using Meadow.Test.Functional.GenericRequests;
using Meadow.Test.Functional.Models;

namespace Meadow.Test.Functional.TestEnvironment;

public interface IPersonsEnvironment
{
    string[] Transliterate(params string[] searchTerms);
    MeadowEngine Engine { get; }
    
    CaseData Data { get; }

    FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>> filter = null, int offset = 0,
        int size = 1000, Action<OrderSetBuilder<TModel>> order = null, params string[] searchTerms)
        where TModel : class;

    void Index<TModel>(IEnumerable<TModel> items);


    List<TModel> Update<TModel>(Func<TModel, bool> predicate, Action<TModel> update) where TModel : class, new();
}

public interface IEnvironment
{
    
    string[] Transliterate(params string[] searchTerms);
    MeadowEngine Engine { get; }

    CaseData Data { get; }
    FindPagedRequest<TModel> FindPaged<TModel>(Action<FilterQueryBuilder<TModel>> filter = null, int offset = 0,
        int size = 1000, Action<OrderSetBuilder<TModel>> order = null, params string[] searchTerms)
        where TModel : class;

    void Index<TModel>(IEnumerable<TModel> items);

    List<TModel> Update<TModel>(Func<TModel, bool> predicate, Action<TModel> update) where TModel : class, new();
}