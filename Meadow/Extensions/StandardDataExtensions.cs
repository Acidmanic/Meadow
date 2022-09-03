using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Contracts;

namespace Meadow.Extensions
{
    public static class StandardDataExtensions
    {
        public static List<Record> RelationalToStandard<TModel>(this List<Record> data,
            IRelationalIdentifierToStandardFieldMapper mapper, bool fullTree)
        {
            var map = mapper.MapAddressesByIdentifier<TModel>(fullTree);

            var translated = new List<Record>();

            foreach (var record in data)
            {
                var translatedRecord = record.RelationalToStandard(map);

                translated.Add(translatedRecord);
            }

            return translated;
        }

        public static Record RelationalToStandard<TModel>(this IEnumerable<DataPoint> record,
            IRelationalIdentifierToStandardFieldMapper mapper, bool fullTree)
        {
            var map = mapper.MapAddressesByIdentifier<TModel>(fullTree);

            return record.RelationalToStandard(map);
        }

        private static Record RelationalToStandard(this IEnumerable<DataPoint> record,
            Dictionary<string, FieldKey> toStandardMap)
        {
            var translated = new Record();

            foreach (var dataPoint in record)
            {
                if (toStandardMap.ContainsKey(dataPoint.Identifier))
                {
                    var translatedDatapoint = new DataPoint
                    {
                        Identifier = toStandardMap[dataPoint.Identifier].ToString(),
                        Value = dataPoint.Value
                    };
                    translated.Add(translatedDatapoint);
                }
            }

            return translated;
        }

        public static Record StandardToRelational<TModel>(this IEnumerable<DataPoint> record,
            IRelationalIdentifierToStandardFieldMapper mapper, bool fullTree)
        {
            return StandardToRelational(record, mapper, typeof(TModel), fullTree);
        }


        public static Record StandardToRelational(this IEnumerable<DataPoint> record,
            IRelationalIdentifierToStandardFieldMapper mapper, Type modelType, bool fullTree)
        {
            var map = mapper.MapAddressesByIdentifier(modelType, fullTree).Reverse();

            var translated = new Record();

            foreach (var dataPoint in record)
            {
                var key = FieldKey.Parse(dataPoint.Identifier).ClearIndexes();

                if (map.ContainsKey(key))
                {
                    var translatedDatapoint = new DataPoint
                    {
                        Identifier = map[key].ToString(),
                        Value = dataPoint.Value
                    };
                    translated.Add(translatedDatapoint);
                }
            }

            return translated;
        }
    }
}