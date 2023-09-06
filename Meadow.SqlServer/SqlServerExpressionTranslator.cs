using Meadow.Sql;

namespace Meadow.SqlServer
{
    public class SqlServerExpressionTranslator : SqlExpressionTranslatorBase
    {
        protected override string EscapedSingleQuote => "''";

        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;
    }
}