using System;
using System.IO;

namespace Examples.Common
{
    public class ExampleConnectionString
    {
        /// <summary>
        /// :D
        /// </summary>
        /// <returns>Sql server password</returns>
        public static string ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt()
        {
            var reachedTheEnd = false;

            var currentPath = new DirectoryInfo(".").FullName;

            while (!reachedTheEnd)
            {
                var currentFile = Path.Join(currentPath, "sa-pass");

                if (File.Exists(currentFile))
                {
                    return File.ReadAllText(currentFile).Trim();
                }

                var currentDirectory = new DirectoryInfo(currentPath);

                var parent = currentDirectory.Parent;

                reachedTheEnd = parent == null;

                if (!reachedTheEnd)
                {
                    currentPath = parent.FullName;
                }
            }

            throw new Exception("Please create a text file, named 'sa-pass' " +
                                "containing your password, and put it in the solution directory.");
        }

        public static string GetMySqlConnectionString(string databaseName = "MeadowScratch")
        {
            var password = ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt();

            return
                $"Allow User Variables=True;Server=localhost;Database={databaseName};Uid=root;Pwd='{password.Trim()}';";
        }

        public static string GetSqlServerConnectionString(string databaseName = "MeadowScratch")
        {
            var password = ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt();

            return "Server=localhost;" +
                   "User Id=sa; " +
                   $"Password={password.Trim()};" +
                   $"Database={databaseName}; " +
                   "MultipleActiveResultSets=true";
        }

        public static string GetPostgresConnectionString(string databaseName = "MeadowScratch")
        {
            var password = ReadMyVerySecurePasswordFromGitIgnoredFileSoNoOneSeesIt();

            return $"User ID=postgres;Password='{password}';Host=localhost;Port=5432;" +
                   $"Database={databaseName};";
        }

        public static string GetSqLiteConnectionString(string databaseName = "meadow-scratch")
        {
            return $"Data Source={databaseName}.db";
        }
    }
}