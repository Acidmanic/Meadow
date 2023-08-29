using Meadow.Sql;

namespace Meadow.MySql
{
    public class MySqlFilterQueryTranslator : SqlFilterQueryTranslatorBase
    {
        protected override string EscapedSingleQuote => "\\'";

        protected override bool DoubleQuotesColumnNames => false;
    }
}