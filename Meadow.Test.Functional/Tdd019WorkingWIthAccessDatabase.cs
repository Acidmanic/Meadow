using System;
using System.Data.Odbc;
using System.IO;
using System.Resources;
using Meadow.Test.Functional.TDDAbstractions;
using Microsoft.Data.Sqlite;

namespace Meadow.Test.Functional
{
    public class Tdd019WorkingWIthAccessDatabase : MeadowFunctionalTest
    {
        public override void Main()
        {
            var file = $"/home/diego/{DbName}.db";

            //file = "/home/diego/Downloads/Blank.accdb";

            if (File.Exists(file))
            {
                //File.Create(file);
                // var blanko = BinaryResources.Read("BlankAccessDatabase");
                //
                // File.WriteAllBytes(file,blanko);
                File.Delete(file);
            }

            var connectionString = $"Data Source={file}";

            var query = $"CREATE Table Titles(Id bigint,Title nvarchar(32))";

            ExecuteQuery(query, connectionString);

            query = "insert into Titles (Id,Title) values (1,'Mani');\n" +
                    "insert into Titles (Id,Title) values (2,'Farimehr');";

            ExecuteQuery(query, connectionString);
            
            query = $"CREATE Procedure spReadAllTitles\n" +
                    $"AS\n" +
                    $"select * from Titles";

            var result = ExecuteQuery(query, connectionString);
        }

        private object ExecuteQuery(string query, string connectionString)
        {
            var command = new SqliteCommand(query);

            using (var connection = new SqliteConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
            }

            return null;
        }
    }
}