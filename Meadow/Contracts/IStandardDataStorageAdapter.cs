using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Contracts
{
    public interface IStandardDataStorageAdapter<TToStorageCarrier, TFromStorageCarrier>
    {
        List<TModel> ReadFromStorage<TModel>(TFromStorageCarrier carrier, IFieldMarks fromStorageMarks);

        void WriteToStorage(TToStorageCarrier carrier, IFieldMarks toStorageMarks, ObjectEvaluator evaluator);
    }
}