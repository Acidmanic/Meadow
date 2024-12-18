using System;
using Acidmanic.Utilities.Reflection;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Models;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
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
                creationHeader = "CREATE OR ALTER PROCEDURE";
            }

            return creationHeader + " " + procedureName;
        }

        public override string CreateTablePhrase(RepetitionHandling repetition, string tableName)
        {
            var creationHeader = "CREATE TABLE " + tableName;

            if (repetition == RepetitionHandling.Alter)
            {
                creationHeader = "DROP TABLE IF EXISTS " + tableName + ";" +
                                 "\nCREATE TABLE " + tableName;
            }

            if (repetition == RepetitionHandling.Skip)
            {
                creationHeader = "CREATE TABLE IF NOT EXISTS " + tableName;
            }

            return creationHeader;
        }

        public override TableParameterDefinition TableColumnDefinition(Parameter parameter)
        {
            var definition = parameter.Name + " " + parameter.Type;

            if (parameter.IdentifierStatus.Is(ParameterIdentifierStatus.Unique))
            {
                definition += " NOT NULL PRIMARY KEY";
            }

            if (parameter.IdentifierStatus.Is(ParameterIdentifierStatus.AutoGenerated))
            {
                definition += " AUTOINCREMENT";
            }

            return new TableParameterDefinition(definition,string.Empty);
        }

        public override string TranslatePagination(Parameter offset, Parameter size)
        {
            return $"LIMIT {this.Decorate(size,ParameterUsage.ProcedureBody)}" +
                   $" OFFSET {this.Decorate(offset,ParameterUsage.ProcedureBody)}";
        }

        public override string CreateViewPhrase(RepetitionHandling repetition, string viewName)
        {
            var creationHeader = "CREATE VIEW";

            if (repetition == RepetitionHandling.Alter)
            {
                creationHeader = "DROP VIEW IF EXISTS " + viewName + ";" +
                                 "\nCREATE";
            }

            if (repetition == RepetitionHandling.Skip)
            {
                creationHeader = "CREATE VIEW IF NOT EXISTS";
            }

            return creationHeader + " " + viewName;
        }

        public  override bool DoubleQuotesColumnNames => false;
        public  override bool DoubleQuotesTableNames => false;


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
        
        protected override string EscapedStringValueQuote => "''";

        public SqLiteTranslator(MeadowConfiguration configuration) : base(configuration)
        {
        }
    }
}