using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Contracts;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Test.Functional.FakeEngine
{
    public class FakeAdapter:IStandardDataStorageAdapter<int,int>
    {
        public List<TModel> ReadFromStorage<TModel>(int carrier, IFieldMarks fromStorageMarks,bool fullTree)
        {
            return new List<TModel>();
        }

        public void WriteToStorage(int carrier, IFieldMarks toStorageMarks, ObjectEvaluator evaluator)
        {
            
        }
    }
}