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

        public override string CreateTablePhrase(RepetitionHandling repetition, string tableName)
        {
            var creationHeader = "create table";

            if (repetition == RepetitionHandling.Alter)
            {
                creationHeader = $"drop table if exists \"{tableName}\";" +
                                 $"\ncreate table";
            }

            if (repetition == RepetitionHandling.Skip)
            {
                creationHeader = "create table if not exists";
            }

            return $"{creationHeader} \"{tableName}\"";
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