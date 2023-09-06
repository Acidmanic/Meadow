using Meadow.Sql;

namespace Meadow.SQLite
{
    public class SqLiteExpressionTranslator : SqlExpressionTranslatorBase
    {
        protected override string EscapedSingleQuote => "\\'";

        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;


        protected override string EmptyQuery => "TRUE";
        
        
    }
}