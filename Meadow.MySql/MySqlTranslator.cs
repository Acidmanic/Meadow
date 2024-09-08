using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Sql;

namespace Meadow.MySql
{
    public class MySqlTranslator : SqlTranslatorBase
    {
        public override string CreateProcedurePhrase(RepetitionHandling repetition, string procedureName)
        {
            if (repetition == RepetitionHandling.Alter)
            {
                return "DROP PROCEDURE IF EXISTS " + procedureName + ";" +
                       "\nCREATE PROCEDURE " + procedureName;
            }

            return "CREATE PROCEDURE " + procedureName;
        }

        public override string CreateTablePhrase(RepetitionHandling repetition, string tableName)
        {
            var dropping = "";
            var creation = "CREATE TABLE";

            if (repetition == RepetitionHandling.Alter)
            {
                dropping = "DROP TABLE IF EXISTS " + tableName + ";\n";
            }

            if (repetition == RepetitionHandling.Skip)
            {
                creation = "CREATE TABLE IF NOT EXISTS";
            }

            return $"{dropping}{creation} {tableName}";
        }

        public override string TableColumnDefinition(Parameter parameter)
        {
            var tableColumnDefinition = parameter.Name + " " + parameter.Type;

            tableColumnDefinition += parameter.IdentifierStatus.Is(ParameterIdentifierStatus.Unique) ? " NOT NULL PRIMARY KEY" : "";

            tableColumnDefinition += parameter.IdentifierStatus.Is(ParameterIdentifierStatus.AutoGenerated) ? " AUTO_INCREMENT" : "";

            return tableColumnDefinition;
        }

        public override string CreateViewPhrase(RepetitionHandling repetition, string viewName)
        {
            if (repetition == RepetitionHandling.Alter)
            {
                return "DROP VIEW IF EXISTS " + viewName + ";" +
                       "\nCREATE VIEW " + viewName;
            }

            return "CREATE VIEW " + viewName;
        }

        public override bool DoubleQuotesColumnNames => false;
        public override bool DoubleQuotesTableNames => false;

        public override string ProcedureBodyParameterNamePrefix => "";
        public override string ProcedureDefinitionParameterNamePrefix => "IN ";

        public MySqlTranslator(MeadowConfiguration configuration) : base(new MySqlValueTranslator(configuration.ExternalTypeCasts))
        {
            Configuration = configuration;
        }

        public override string FormatProcedure(string creationPhrase, string parametersPhrase, string bodyContent, string declarations = "", string returnDataTypeName = "")
        {
            if (ParameterLessProcedureDefinitionParentheses || !string.IsNullOrWhiteSpace(parametersPhrase))
            {
                parametersPhrase = $"({parametersPhrase})";
            }

            return creationPhrase + parametersPhrase + "\nBEGIN\n" + bodyContent + "\nEND;\n";
        }
    }
}