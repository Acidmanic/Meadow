using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.SqlServer
{
    public class SqlServerTranslator : SqlTranslatorBase
    {
        public override string CreateProcedurePhrase(RepetitionHandling repetition, string procedureName)
        {
            if (repetition == RepetitionHandling.Alter)
            {
                return $"CREATE OR ALTER PROCEDURE {procedureName}";
            }

            return $"CREATE PROCEDURE {procedureName}";
        }

        public override string CreateTablePhrase(RepetitionHandling repetition, string tableName)
        {
            var creationHeader = "CREATE TABLE";

            if (repetition == RepetitionHandling.Alter)
            {
                creationHeader = "DROP TABLE IF EXISTS " + tableName +
                                 "\nCREATE TABLE";
            }
            else if (repetition == RepetitionHandling.Skip)
            {
                creationHeader = $"IF OBJECT_ID(N'{tableName}', N'U') IS NULL" +
                                 "\nCREATE TABLE";
            }

            return $"{creationHeader} {tableName}";
        }

        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;

        public SqlServerTranslator(MeadowConfiguration configuration)
        :base(new SqlServerValueTranslator(configuration.ExternalTypeCasts))
        {
            Configuration = configuration;
        }
    }
}