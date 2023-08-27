using Meadow.Sql;

namespace Meadow.SqlServer
{
    public class SqlServerFilterQueryTranslator : SqlFilterQueryTranslatorBase
    {
        protected override string EscapedSingleQuote => "''";

        protected override bool DoubleQuotesColumnNames => false;
    }
}