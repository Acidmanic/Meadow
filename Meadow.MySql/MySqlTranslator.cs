using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.MySql
{
    public class MySqlTranslator : SqlTranslatorBase
    {
        public override string CreateProcedurePhrase(RepetitionHandling repetition, string procedureName)
        {
            if (repetition == RepetitionHandling.Alter)
            {
                return "DROP PROCEDURE IF EXISTS " + procedureName + ";" +
                       "\nCREATE PROCEDURE "+procedureName;
            }

            return "CREATE PROCEDURE "+procedureName;
        }

        public override string CreateTablePhrase(RepetitionHandling repetition, string tableName)
        {
            var dropping = "";
            var creation = "CREATE TABLE";

            if (repetition == RepetitionHandling.Alter)
            {
                dropping = "DROP TABLE IF EXISTS " + tableName + ";\n";
            }

            if (repetition == RepetitionHandling.Skip)
            {
                creation = "CREATE TABLE IF NOT EXISTS";
            }

            return $"{dropping}{creation} {tableName}";
        }

        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;

        public MySqlTranslator(MeadowConfiguration configuration):base(new MySqlValueTranslator(configuration.ExternalTypeCasts))
        {
            Configuration = configuration;
        }
    }
}