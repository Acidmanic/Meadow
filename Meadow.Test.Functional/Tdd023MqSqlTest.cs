using Meadow.Configuration;
using Meadow.Log;
using Meadow.SqlServer;
using Meadow.Test.Functional.TDDAbstractions;
using MySql.Data.MySqlClient;

namespace Meadow.Test.Functional
{
    public class Tdd023MqSqlTest:MeadowFunctionalTest
    {
        public override void Main()
        {
            var connectionString = "Server=localhost;Database=MeadowScratch;Uid=sa;Pwd='never54aga.1n'; ";
            
            var query = $"CREATE Table Titles(Id bigint,Title nvarchar(32))";
            
            ExecuteQuery(query, connectionString);

            query = "insert into Titles (Id,Title) values (1,'Mani');\n" +
                    "insert into Titles (Id,Title) values (2,'Farimehr');";
            
            ExecuteQuery(query, connectionString);
            
            query = $"CREATE Procedure spReadAllTitles()\n" +
                    $"BEGIN\n" +
                    $"select * from Titles;\n" +
                    $"END";
            
            var result = ExecuteQuery(query, connectionString);
        }
        
        private object ExecuteQuery(string query, string connectionString)
        {
            var command = new MySqlCommand(query);

            using (var connection = new MySqlConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
            }

            return null;
        }
    }
}