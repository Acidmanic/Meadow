using System.Linq;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Extensions;
using Meadow.Sql;

namespace Meadow.MySql
{
    public class MySqlTranslator : SqlTranslatorBase
    {
        
        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;

        public MySqlTranslator(MeadowConfiguration configuration):base(new MySqlValueTranslator(configuration.ExternalTypeCasts))
        {
            Configuration = configuration;
        }
    }
}