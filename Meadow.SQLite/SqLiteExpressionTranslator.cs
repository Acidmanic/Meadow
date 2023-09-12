using System;
using Acidmanic.Utilities.Reflection;
using Meadow.Extensions;
using Meadow.Sql;

namespace Meadow.SQLite
{
    public class SqLiteExpressionTranslator : SqlExpressionTranslatorBase
    {
        protected override string EscapedSingleQuote => "\\'";

        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;


        protected override string EmptyConditionExpression => "TRUE";


        protected override string EmptyOrderExpression(Type entityType, bool fullTree)
        {
            var nc = Configuration.GetNameConvention(entityType);

            var table = fullTree ? nc.FullTreeViewName : nc.TableName;

            if (DoubleQuotesTableNames)
            {
                table = $"\"{table}\"";
            }

            var idLeaf = TypeIdentity.FindIdentityLeaf(entityType);

            if (idLeaf == null)
            {
                return "ROWID";
            }

            var headlessKey = HeadLess(idLeaf.GetFullName());
            var fieldName = TranslateFieldName(entityType, headlessKey, fullTree);

            return table + '.' + fieldName;


        }

        private string HeadLess(string key)
        {
            var st = key.IndexOf(".", 0, StringComparison.Ordinal);

            if (st > -1)
            {
                return key.Substring(st + 1, key.Length - st - 1);
            }

            return key;
        }
    }
}