using System;
using System.Collections.Generic;
using System.IO;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Acidmanic.Utilities.Results;

namespace Meadow.Test.Functional.Utility
{
    public class CsvData
    {

        private List<Type> Types { get; set; } = new List<Type>();


        public List<Record> Data { get; private set; } = new List<Record>();

        public CsvData()
        {
            
        }


        protected void AddColumnType<T>()
        {
            AddColumnType(typeof(T));
        }
        protected void AddColumnType(Type type)
        {
            Types.Add(type);
        }

        public void Load(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            var firstLine = true;

            var identifiers = new string[] { };
            
            this.Data.Clear();
            
            foreach (var line in lines)
            {
                var record = new Record();

                var segments = line.Split(",");

                if (firstLine)
                {
                    identifiers = segments;
                    firstLine = false;
                }
                else
                {
                    for (int i = 0; i < identifiers.Length; i++)
                    {
                        var value = GetValueFor(i, segments);
                        
                        record.Add(identifiers[i],value);
                    }
                }
                
                this.Data.Add(record);
            }
        }

        private object GetValueFor(int columnIndex, string[] segments)
        {
            if (columnIndex >= segments.Length)
            {
                return null;
            }
            var type = GetTypeFor(columnIndex);

            if (type == typeof(string))
            {
                return segments[columnIndex];
            }

            var parsed = TryParse(type, segments[columnIndex]);

            if (parsed)
            {
                return parsed.Value;
            }

            return null;
        }

        private Result<object> TryParse(Type type, string segment)
        {
            var parameterTypes = new Type[]{ typeof(string)};

            var parseMethod = type.GetMethod("Parse",parameterTypes);

            if (parseMethod != null)
            {
                try
                {
                    var parsed = parseMethod.Invoke(null, new object[] { segment });

                    return new Result<object>(true, parsed);
                }
                catch (Exception _) { }
            }

            return false;
        }


        private Type GetTypeFor(int columnIndex)
        {
            if (columnIndex >= 0 && columnIndex < Types.Count)
            {
                return Types[columnIndex];
            }

            return typeof(string);
        }

    }
}