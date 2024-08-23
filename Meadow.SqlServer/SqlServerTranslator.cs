using Meadow.Configuration;
using Meadow.Sql;

namespace Meadow.SqlServer
{
    public class SqlServerTranslator : SqlTranslatorBase
    {
        
        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;

        public SqlServerTranslator(MeadowConfiguration configuration)
        :base(new SqlServerValueTranslator(configuration.ExternalTypeCasts))
        {
            Configuration = configuration;
        }
    }
}