using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd000RetrieveDataFromDatabaseTest:IFunctionalTest
    {
        
        public class Tag
        {
            public long PropertyId { get; set; }

            public long ProductClassId { get; set; }
        }
        
        
        public void Main()
        {
            var tags = ReadAll();

            foreach (var tag in tags)
            {
                Console.WriteLine("Tag: " + tag.PropertyId + "," + tag.ProductClassId);
            }
        }
        
        
        private IEnumerable<Tag> ReadAll()
        {
            var result = new List<Tag>();

            var jsonObjects = new List<string>();

            using (var connection =
                new SqlConnection(
                    "Server=localhost;User Id=sa; Password=never54aga.1n;Database=MeadowDatabase; MultipleActiveResultSets=true")
            )
            {
                var command = new SqlCommand("spReadAllTags", connection);

                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                var dataReader = command.ExecuteReader();

                Console.WriteLine("-----------------------------------");

                while (dataReader.Read())
                {
                    var tag = new Tag
                    {
                        PropertyId = Convert.ToInt64(dataReader["PropertyId"]),
                        ProductClassId = Convert.ToInt64(dataReader["ProductClassId"])
                    };

                    var line = "";
                    var json = "{";
                    var sep = "";
                    for (var index = 0; index < dataReader.FieldCount; index++)
                    {
                        var name = dataReader.GetName(index);
                        var value = dataReader[name];
                        json += sep + "\"" + name + "\"" + ": \"" + value + "\"";
                        sep = ",";
                        line += name + ": " + value + ",";
                    }

                    json += "}";

                    jsonObjects.Add(json);
                    Console.WriteLine(line);
                    result.Add(tag);
                }

                Console.WriteLine("-----------------------------------");

                var fullJson = "";
                if (jsonObjects.Count == 1)
                {
                    fullJson = jsonObjects[0];
                }
                else if (jsonObjects.Count > 1)
                {
                    fullJson = "[";
                    var sep = "";
                    foreach (var j in jsonObjects)
                    {
                        fullJson += sep + j;
                        sep = ",";
                    }

                    fullJson += "]";
                }

                Console.WriteLine(fullJson);

                Console.WriteLine("-----------------------------------");
            }

            return result;
        }
    }
}