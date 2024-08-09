using Meadow.Configuration;
using Meadow.Sql;

namespace Meadow.SqlServer
{
    public class SqlServerExpressionTranslator : SqlExpressionTranslatorBase
    {
        
        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;

        public SqlServerExpressionTranslator(MeadowConfiguration configuration)
        :base(new SqlServerValueTranslator(configuration.ExternalTypeCasts))
        {
            Configuration = configuration;
        }
    }
}