using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Contracts;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Test.Functional.FakeEngine
{
    public class FakeAdapter : IStandardDataStorageAdapter<int, int>
    {
        public List<TModel> ReadFromStorage<TModel>(int carrier, IFieldMarks<TModel> fromStorageMarks,
            bool fullTreeRead)
        {
            return new List<TModel>();
        }

        public void WriteToStorage<TModel>(int carrier, IFieldMarks<TModel> toStorageMarks, ObjectEvaluator evaluator)
        {
        }
    }
}