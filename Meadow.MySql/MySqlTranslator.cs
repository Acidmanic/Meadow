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

        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;

        public MySqlTranslator(MeadowConfiguration configuration):base(new MySqlValueTranslator(configuration.ExternalTypeCasts))
        {
            Configuration = configuration;
        }
    }
}