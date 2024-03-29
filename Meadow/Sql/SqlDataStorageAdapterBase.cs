using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Acidmanic.Utilities.Reflection.Utilities;
using Meadow.Casting;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;
using Microsoft.Extensions.Logging;

namespace Meadow.Sql
{
    public abstract class SqlDataStorageAdapterBase : IStandardDataStorageAdapter<IDbCommand, IDataReader>
    {
        private readonly ILogger _logger;
        private readonly MeadowBuiltinCastList _castings;

        protected SqlDataStorageAdapterBase(MeadowConfiguration configuration, ILogger logger)
        {
            FieldNameDelimiter = configuration.DatabaseFieldNameDelimiter;
            DataOwnerNameProvider = configuration.TableNameProvider;
            _logger = logger;
            RelationalIdentifierToStandardFieldMapper = configuration.GetRelationalStandardMapper();
            _castings = new MeadowBuiltinCastList(configuration.ExternalTypeCasts);
        }

        public char FieldNameDelimiter { get; }

        public IDataOwnerNameProvider DataOwnerNameProvider { get; }

        public IRelationalIdentifierToStandardFieldMapper RelationalIdentifierToStandardFieldMapper { get; }

        public List<TModel> ReadFromStorage<TModel>(IDataReader carrier, IFieldInclusion fromStorageInclusion, bool fullTreeRead)
        {
            var storageData = ReadAllRecords(carrier);

            storageData = Filter(storageData, fromStorageInclusion);

            var unIndexedStandard = storageData.RelationalToStandard<TModel>
                (RelationalIdentifierToStandardFieldMapper, fullTreeRead);

            unIndexedStandard = CastBackAlteredTypes<TModel>(unIndexedStandard);

            var accumulator = new StandardIndexAccumulator<TModel>(_logger);

            accumulator.PassAll(unIndexedStandard);

            var indexedStandard = accumulator.Records;

            List<TModel> results = new List<TModel>();

            using (var s = CastScope.Create(_castings))
            {
                foreach (var record in indexedStandard)
                {
                    var evaluator = new ObjectEvaluator(typeof(TModel));

                    evaluator.LoadStandardData(record);

                    var recordObject = evaluator.As<TModel>();

                    if (recordObject != null)
                    {
                        results.Add(recordObject);
                    }
                }
            }

            return results;
        }

        private List<Record> CastBackAlteredTypes<TModel>(List<Record> records)
        {
            var data = new List<Record>();

            var evaluator = new ObjectEvaluator(typeof(TModel));

            foreach (var record in records)
            {
                var castedRecord = new Record();

                foreach (var dataPoint in record)
                {
                    var value = dataPoint.Value;

                    if (value != null)
                    {
                        var address = dataPoint.Identifier;

                        var node = evaluator.Map.NodeByAddress(address);

                        var expectedType = node.Type;

                        var alterCast = expectedType.GetCustomAttribute<AlteredTypeAttribute>();

                        var actualType = value.GetType();

                        if (alterCast != null && alterCast.AlternativeType == actualType)
                        {
                            value = value.CastTo(expectedType);
                        }
                    }

                    castedRecord.Add(dataPoint.Identifier, value);
                }

                data.Add(castedRecord);
            }

            return data;
        }

        public virtual void WriteToStorage(IDbCommand command, IFieldInclusion toStorageInclusion,
            ObjectEvaluator evaluator)
        {
            var standardData = evaluator.ToStandardFlatData(o =>
                o.DirectLeavesOnly().UseAlternativeTypes());

            var includedData = standardData.Where(dp => toStorageInclusion.IsIncluded(dp.Identifier));

            var data = includedData.StandardToRelational(
                RelationalIdentifierToStandardFieldMapper,
                evaluator.RootNode.Type, false);


            WriteAllToCommand(data, command);
        }

        /// <summary>
        /// This method will be called after data being filtered and standardized and ready to be written into command.
        /// Default implementation will make call to <code>WriteIntoCommand(DataPoint dataPoint, IDbCommand command)</code>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="command"></param>
        protected virtual void WriteAllToCommand(List<DataPoint> data, IDbCommand command)
        {
            data.ForEach(dataPoint => WriteIntoCommand(dataPoint, command));
        }

        protected abstract void WriteIntoCommand(DataPoint dataPoint, IDbCommand command);


        private Record Filter(Record record, IFieldInclusion filter)
        {
            var filteredRecord =
                record.Where(dp => filter.IsIncluded(dp.Identifier));

            return new Record(filteredRecord);
        }

        private List<Record> Filter(List<Record> records, IFieldInclusion filter)
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