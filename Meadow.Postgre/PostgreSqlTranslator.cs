using Meadow.Configuration;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.Postgre
{
    public class PostgreSqlTranslator : SqlTranslatorBase
    {
        public override string CreateProcedurePhrase(RepetitionHandling repetition, string procedureName)
        {
            var creationHeader = "create";

            if (repetition == RepetitionHandling.Alter)
            {
                creationHeader = "create or replace";
            }
            
            return creationHeader + $" \"{procedureName}\"";
        }

        protected override bool DoubleQuotesColumnNames => true;
        protected override bool DoubleQuotesTableNames => true;

        protected override string NotEqualOperator => "<>";

        public PostgreSqlTranslator(MeadowConfiguration configuration)
            : base(new PostgreValueTranslator(configuration.ExternalTypeCasts))
        {
            Configuration = configuration;
        }
    }
}