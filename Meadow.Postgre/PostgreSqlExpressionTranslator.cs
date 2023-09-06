using System;
using System.Collections.Generic;
using System.Text;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection;
using Meadow.Contracts;
using Meadow.Sql;
using Microsoft.Extensions.Logging;

namespace Meadow.Postgre
{
    public class PostgreSqlExpressionTranslator : SqlExpressionTranslatorBase
    {
        protected override string EscapedSingleQuote => "\\'";

        protected override bool DoubleQuotesColumnNames => true;
        protected override bool DoubleQuotesTableNames => true;
    }
}