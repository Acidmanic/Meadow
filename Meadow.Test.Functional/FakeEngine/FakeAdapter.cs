using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Contracts;
using Meadow.Requests.FieldInclusion;

namespace Meadow.Test.Functional.FakeEngine
{
    public class FakeAdapter : IStandardDataStorageAdapter<int, int>
    {
        public List<TModel> ReadFromStorage<TModel>(int carrier, IFieldInclusion<TModel> fromStorageInclusion,
            bool fullTreeRead)
        {
            return new List<TModel>();
        }

        public void WriteToStorage<TModel>(int carrier, IFieldInclusion<TModel> toStorageInclusion, ObjectEvaluator evaluator)
        {
        }
    }
}