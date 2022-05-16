using System;
using System.Collections.Generic;

namespace Meadow.Utility
{
    public class ConnectionStringParser
    {
        public Dictionary<string, string> Parse(string connectionString)
        {
            var segments = connectionString.Split(";", StringSplitOptions.RemoveEmptyEntries);

            var result = new Dictionary<string, string>();

            foreach (var item in segments)
            {
                var keyValue = item.Split("=");

                if (keyValue != null && keyValue.Length == 2)
                {
                    result.Add(keyValue[0], keyValue[1]);
                }
            }

            return result;
        }

        public string CreateConnectionString(Dictionary<string, string> valuesMap)
        {
            var connectionString = "";

            foreach (var item in valuesMap)
            {
                connectionString += item.Key + "=" + item.Value + ";";
            }

            return connectionString;
        }
    }
}