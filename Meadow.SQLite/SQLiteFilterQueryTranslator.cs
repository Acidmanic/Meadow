using System;
using System.Collections.Generic;
using System.Text;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection;
using Meadow.Contracts;
using Meadow.Sql;
using Microsoft.Extensions.Logging;

namespace Meadow.SQLite
{
    public class SqLiteFilterQueryTranslator : SqlFilterQueryTranslatorBase
    {
        protected override string EscapedSingleQuote => "\\'";

        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;


        protected override string EmptyQuery => "TRUE";
        
        
    }
}