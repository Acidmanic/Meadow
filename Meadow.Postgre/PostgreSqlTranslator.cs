using System;
using System.Collections.Generic;
using System.Text;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Sql;
using Microsoft.Extensions.Logging;

namespace Meadow.Postgre
{
    public class PostgreSqlTranslator : SqlTranslatorBase
    {
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