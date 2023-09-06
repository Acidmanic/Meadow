using System.Linq;
using Meadow.Extensions;
using Meadow.Sql;

namespace Meadow.MySql
{
    public class MySqlFilterQueryTranslator : SqlFilterQueryTranslatorBase
    {
        protected override string EscapedSingleQuote => "\\'";

        protected override bool DoubleQuotesColumnNames => false;

        public string TranslateSearchTerm<TEntity>(string[] searchSegments)
        {

            var nc = Configuration.GetNameConvention<TEntity>();

            var searchIndexTable = nc.SearchIndexTableName;

            var columnFullName = searchIndexTable + ".IndexCorpus";

            return string.Join(" OR ", searchSegments.Select(
                s => $"{columnFullName} like '%{s}%'"));

        }
    }
}