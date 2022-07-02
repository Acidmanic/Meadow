using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Contracts;
using Meadow.Requests.FieldManipulation;

namespace Meadow.Sql
{
    public abstract class SqlDataStorageAdapterBase : IStandardDataStorageAdapter<IDbCommand, IDataReader>
    {
        public List<TModel> ReadFromStorage<TModel>(IDataReader carrier, IFieldMarks fromStorageMarks)
        {
            var storageData = ReadAllRecords(carrier);

            storageData = Filter(storageData, fromStorageMarks);

            var standardData = new SqlStandardDataTranslator().TranslateFromStorage(storageData, typeof(TModel));

            List<TModel> results = new List<TModel>();

            foreach (var record in standardData)
            {
                var evaluator = new ObjectEvaluator(typeof(TModel));

                evaluator.LoadStandardData(record);

                var recordObject = evaluator.As<TModel>();

                if (recordObject != null)
                {
                    results.Add(recordObject);
                }
            }

            return results;
        }

        public virtual void WriteToStorage(IDbCommand command, IFieldMarks toStorageMarks, ObjectEvaluator evaluator)
        {
            var standardData = new Record(evaluator.ToStandardFlatData()
                // Select Only Direct-Leaves
                .Where(dp =>
                {
                    var node = evaluator.Map.NodeByAddress(dp.Identifier);

                    return node.IsLeaf && node.Parent == evaluator.RootNode;
                }));

            List<DataPoint> data = new SqlStandardDataTranslator().TranslateToStorage(standardData, evaluator);

            foreach (var dataPoint in data)
            {
                if (toStorageMarks.IsIncluded(dataPoint.Identifier))
                {
                    WriteIntoCommand(dataPoint, command);
                }
            }
        }

        protected abstract void WriteIntoCommand(DataPoint dataPoint, IDbCommand command);


        private Record Filter(Record record, IFieldMarks filter)
        {
            var filteredRecord =
                record.Where(dp => filter.IsIncluded(dp.Identifier));

            return new Record(filteredRecord);
        }

        private List<Record> Filter(List<Record> records, IFieldMarks filter)
        {
            var filteredRecords = records.Select(r => Filter(r, filter));

            return new List<Record>(filteredRecords);
        }

        private List<Record> ReadAllRecords(IDataReader dataReader)
        {
            var drFields = EnumFields(dataReader);

            var records = new List<Record>();

            while (dataReader.Read())
            {
                // For each Record
                var record = new Record();
                // Read all the record data cells
                foreach (var field in drFields)
                {
                    var value = dataReader[field];

                    if (!(value is DBNull) && value != null)
                    {
                        var datapoint = new DataPoint
                        {
                            Identifier = field,
                            Value = dataReader[field]
                        };
                        record.Add(datapoint);
                    }
                }

                // Store data in memory for further processing
                records.Add(record);
            }

            return records;
        }


        private List<string> EnumFields(IDataReader dataReader)
        {
            var result = new List<string>();

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                result.Add(dataReader.GetName(i));
            }

            return result;
        }
    }
}