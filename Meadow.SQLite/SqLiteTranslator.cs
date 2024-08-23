using System;
using Acidmanic.Utilities.Reflection;
using Meadow.Configuration;
using Meadow.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Sql;

namespace Meadow.SQLite
{
    public class SqLiteTranslator : SqlTranslatorBase
    {
        public override string CreateProcedurePhrase(RepetitionHandling repetition, string procedureName)
        {
            var creationHeader = "CREATE PROCEDURE";

            if (repetition == RepetitionHandling.Skip)
            {
                creationHeader = "CREATE IF NOT EXISTS";
            }

            if (repetition == RepetitionHandling.Alter)
            {
                creationHeader = "CREATE OR ALTER";
            }

            return creationHeader + " " + procedureName;
        }

        protected override bool DoubleQuotesColumnNames => false;
        protected override bool DoubleQuotesTableNames => false;


        protected override string EmptyConditionExpression => "TRUE";


        public SqLiteTranslator(MeadowConfiguration configuration)
            : base(new SqLiteValueTranslator(configuration.ExternalTypeCasts))
        {
            Configuration = configuration;
        }
        
        
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